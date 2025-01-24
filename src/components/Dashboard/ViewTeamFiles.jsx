import React, { useState, useEffect } from "react";
import "./Dashboard.css";
import { createFile, getTeamFiles, mediaUrl } from "../../config";
import SearchIcon from "@mui/icons-material/Search";
import NotificationsNoneIcon from "@mui/icons-material/NotificationsNone";
import { Box, Tabs, Typography } from "@mui/material";
import { Folder } from "@mui/icons-material";
import Modal from "@mui/material/Modal";
import Tab from "@mui/material/Tab";
import TabPanel from "@mui/lab/TabPanel";
import TabContext from "@mui/lab/TabContext";
import TabList from "@mui/lab/TabList";
import { Button, Card } from "@mui/material";
import FilterAltOutlinedIcon from "@mui/icons-material/FilterAltOutlined";
import Popover from "@mui/material/Popover";
import ArticleIcon from "@mui/icons-material/Article";
import BorderAllIcon from "@mui/icons-material/BorderAll";
import CoPresentIcon from "@mui/icons-material/CoPresent";
import PictureAsPdfIcon from "@mui/icons-material/PictureAsPdf";
import FileDownloadOutlinedIcon from "@mui/icons-material/FileDownloadOutlined";
import FileUploadIcon from "@mui/icons-material/FileUpload";
import FolderCopyIcon from "@mui/icons-material/FolderCopy";
import FormControl from "@mui/material/FormControl";
import TextField from "@mui/material/TextField";
import CardContent from "@mui/material/CardContent";
import moment from "moment";
import { DataGrid } from "@mui/x-data-grid";
import { useNavigate, useLocation } from "react-router-dom";
// import { mediaUrl } from "../../config.js";
import IconButton from "@mui/material/IconButton";
import spinner from "../../images/spinner.gif";
import FolderSharedOutlinedIcon from "@mui/icons-material/FolderSharedOutlined";
import LockIcon from "@mui/icons-material/Lock";
import Divider from "@mui/material/Divider";
import CloseIcon from "@mui/icons-material/Close";
import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import GroupOutlinedIcon from "@mui/icons-material/GroupOutlined";
import SettingsSuggestIcon from "@mui/icons-material/SettingsSuggest";
import DeleteOutlineOutlinedIcon from "@mui/icons-material/DeleteOutlineOutlined";
import TimelineOutlinedIcon from "@mui/icons-material/TimelineOutlined";
import TFDSettings from "../Settings/TFDSettings.jsx";
import Members from "../Settings/Members.jsx";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: "90%",
  height: "90%",
  bgcolor: "background.paper",
  border: "1px solid #000",
  boxShadow: 24,
  p: 4,
};

//** Icons for file */
const displayIcons = (ext) => {
  switch (ext) {
    case ".docx":
    case ".doc":
      return <ArticleIcon className="icon" sx={{ color: "#064d93", mr: 3 }} />;
    case ".xlsx":
    case ".xls":
      return (
        <BorderAllIcon className="icon" sx={{ color: "#42aa47", mr: 3 }} />
      );
    case ".pptx":
    case ".ppt":
      return (
        <CoPresentIcon className="icon" sx={{ color: "#d74b23", mr: 3 }} />
      );
    case ".pdf":
      return (
        <PictureAsPdfIcon className="icon" sx={{ color: "#b20a01", mr: 3 }} />
      );
    default:
      return;
  }
};

//** handle files columns display */
const fileColumns = [
  { field: "id", headerName: "ID", flex: 0.1 },
  {
    field: "fileName",
    headerName: "Name",
    flex: 4,
    renderCell: (params) => (
      <Typography className="popoverContent">
        {displayIcons(params.row.extension)}
        {params.value}
      </Typography>
    ),
  },
  { field: "displayName", headerName: "Owner", flex: 2 },
  {
    field: "createdOnDate",
    headerName: "Date created",
    flex: 2,
    valueGetter: (params) => {
      // Assuming createdOnDate is a string containing the date
      const formattedDate = moment(params).format("MMMM Do YYYY, h:mm:ss a");
      return formattedDate;
    },
  },
  {
    field: "location",
    headerName: "",
    flex: 2,
    renderCell: (params) => (
      <Typography className="popoverContent">
        {/* <FileDownloadOutlinedIcon className="icon" sx={{ color: "#064d93", mr: 3 }} /> */}
        <a
          href={`${mediaUrl}/${params.row.location}`}
          target="_self"
          rel="noopener noreferrer"
          style={{ textDecoration: "none", color: "inherit" }}
        ></a>
        <IconButton
          component="a"
          href={`${mediaUrl}/${params.row.location}`}
          download={params.value}
        >
          <FileDownloadOutlinedIcon />
        </IconButton>
      </Typography>
    ),
  },
];

const ViewTeamFiles = () => {
  const [openmodal, setOpenModal] = React.useState(false);
  const [value, setValue] = React.useState("TeamFolderDetails");
  const [openmodaldoc, setOpenModalDoc] = React.useState(false);
  const [getFolderId, setGetFolderId] = useState("");
  const [foldername, setFolderName] = useState("");
  const [perm, setPerm] = useState("");
  const [files, setFiles] = useState([]);
  const [file, setFile] = useState(null);
  const [showImg, setShowImg] = useState(true);
  const [data, setData] = useState(null); // New state variable for data

  const navigate = useNavigate();
  const location = useLocation();

  const token = sessionStorage.getItem("token");
  if (token == null || token === "") {
    navigate("/login");
  }

  useEffect(() => {
    //{
    const { folderid, folderName, p: permission } = location.state;
    setGetFolderId(folderid);
    setPerm(Number(permission));
    setFolderName(folderName);

    const js = JSON.parse(token);
    const dataObj = {
      isSuperUser: js[0].isSuperUser,
      datatoken: js[0].token,
      datasecretkey: js[0].secretKey,
      datacreatedbyuserid: js[0].empId,
    };
    setData(dataObj); // Set data in state
    fetchFiles(folderid, dataObj);
    setShowImg(false);
    //}
  }, [location.state, navigate]);

  const fetchFiles = async (folderId, dataObj) => {
    try {
      const send_values = {
        Token: dataObj.datatoken,
        SecretKey: dataObj.datasecretkey,
        FolderId: folderId,
        CreatedByUserId: dataObj.datacreatedbyuserid,
      };

      const response = await fetch(getTeamFiles, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(send_values),
      });

      if (response.statusText === "OK") {
        const data = await response.json();

        if (Array.isArray(data)) {
          setFiles(data);
        } else {
          alert("API response is not an array:", data);
        }
      } else {
        alert(
          `Access to the folder was denied due to ${response.statusText} credentials.`
        );
      }
    } catch (err) {
      alert(`Error retrieving data: ${err}`);
    }
  };

  // Popover functions
  const [anchorEl, setAnchorEl] = useState(null);

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);
  const id = open ? "simple-popover" : undefined;

  // Modal functions
  const handleOpenModal = () => setOpenModal(true);
  const handleCloseModal = () => setOpenModal(false);
  const handleOpenModalDoc = () => setOpenModalDoc(true);
  const handleCloseModalDoc = () => setOpenModalDoc(false);
  const handleDocumentClick = () => {
    handleOpenModalDoc();
  };

  // Tab change function
  const handleChange = (event, newValue) => {
    setValue(newValue);
  };

    //** Document, Spreadsheet and Excel Upload.
  //** This function's job is to upload MS Office Files  */
  //** function to create new folder */
  //Create New Team
  const ManageSettings = () => {
    handleOpenModal();
  };
  
  const ManageFolderSettings = (
    <div>
      <Modal
        open={openmodal}
        onClose={handleCloseModal}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box
          sx={{
            ...style,
            display: "flex",
            flexDirection: "column",
          }}
        >
          <div
            style={{
              display: "flex",
              flexDirection: "row",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <div
              style={{
                width: "100%",
                marginBottom: "5px",
                display: "flex",
                flexDirection: "row",
                alignContent: "flex-start",
                alignItems: "center",
              }}
            >
              <FolderSharedOutlinedIcon sx={{ marginRight: "10px" }} />
              <Typography>{foldername}</Typography>

              {perm === 1 ? (
                <LockIcon
                  sx={{
                    fontSize: "14px",
                    color: "#000",
                    marginLeft: "7px",
                  }}
                />
              ) : null}
              {data && data.isSuperUser ? (
                <Button
                  variant="contained"
                  onClick={handleClick}
                  sx={{
                    mt: 0.01,
                    backgroundColor: "#212234",
                    textTransform: "capitalize",
                    marginLeft: "7px",
                    height: "20px",
                  }}
                  size="small"
                  type="submit"
                >
                  Admin
                </Button>
              ) : null}
            </div>

            <CloseIcon
              sx={{
                display: "flex",
                justifyContent: "flex-end",
                cursor: "pointer",
                fontSize: "30px",
                fontWeight: 300,
              }}
              onClick={handleCloseModal}
            />
          </div>

          <Divider sx={{ background: "#D3D3D3" }} />
          <Card sx={{ border: 0, boxShadow: "none" }}>
            <CardContent>
              <div
                style={{
                  width: "100%",
                  display: "flex",
                  flexDirection: "column",
                  alignItems: "center",
                  justifyContent: "center",
                  textAlign: "center",
                }}
              >
                <TabContext value={value}>
                  <div
                    style={{
                      margin: "20px auto",
                      borderBottom: "1px solid #D3D3D3", // Add bottom border
                      width: "100%",
                      display: "flex",
                      flexDirection: "column",
                      alignItems: "center",
                      justifyContent: "center",
                      textAlign: "center",
                    }}
                  >
                    <TabList
                      onChange={handleChange}
                      aria-label="lab API tabs example"
                      sx={{
                        "& .MuiTabs-indicator": {
                          backgroundColor: "green", // Custom color for the indicator
                        },
                      }}
                    >
                      <Tab
                        style={{
                          fontSize: "14px",
                          textTransform: "capitalize",
                          color:
                            value === "TeamFolderDetails" ? "green" : "#000", // Custom color for the text
                        }}
                        icon={<InfoOutlinedIcon sx={{ fontWeight: 300 }} />}
                        iconPosition="top"
                        label="Team Folder Details"
                        value="TeamFolderDetails"
                      />
                      <Tab
                        style={{
                          fontSize: "14px",
                          textTransform: "capitalize",
                          color: value === "Members" ? "green" : "#000", // Custom color for the text
                        }}
                        icon={<GroupOutlinedIcon sx={{ fontWeight: 300 }} />}
                        iconPosition="top"
                        label="Members"
                        value="Members"
                      />
                      <Tab
                        style={{
                          fontSize: "14px",
                          textTransform: "capitalize",
                          color: value === "Settings" ? "green" : "#000", // Custom color for the text
                        }}
                        icon={<SettingsSuggestIcon sx={{ fontWeight: 300 }} />}
                        iconPosition="top"
                        label="Settings"
                        value="Settings"
                      />
                      <Tab
                        style={{
                          fontSize: "14px",
                          textTransform: "capitalize",
                          color: value === "Trash" ? "green" : "#000", // Custom color for the text
                        }}
                        icon={
                          <DeleteOutlineOutlinedIcon sx={{ fontWeight: 300 }} />
                        }
                        iconPosition="top"
                        label="Trash"
                        value="Trash"
                      />
                      <Tab
                        style={{
                          fontSize: "14px",
                          textTransform: "capitalize",
                          color: value === "Activity" ? "green" : "#000", // Custom color for the text
                        }}
                        icon={<TimelineOutlinedIcon sx={{ fontWeight: 300 }} />}
                        iconPosition="top"
                        label="Activity"
                        value="Activity"
                      />
                    </TabList>
                  </div>
                  <div style={{ width: "75%" }}>
                    <TabPanel value="TeamFolderDetails">
                      <TFDSettings folderid={getFolderId} />
                    </TabPanel>
                    <TabPanel value="Members">
                      <Members folderid={getFolderId} />
                    </TabPanel>
                    <TabPanel value="Settings">
                      <div>4</div>
                    </TabPanel>
                    <TabPanel value="Trash">
                      <div>3</div>
                    </TabPanel>
                    <TabPanel value="Activity">
                      <div>5</div>
                    </TabPanel>
                  </div>
                </TabContext>
              </div>
            </CardContent>
          </Card>
        </Box>
      </Modal>
    </div>
  );

  return (
    <div>
      {showImg ? (
        <img
          src={spinner}
          alt="spinner"
          style={{
            width: "5%",
            height: "5%",
            marginTop: "100px",
            alignContent: "center",
          }}
        />
      ) : (
        <div
          className="secondRowdiv"
          style={{
            margin: "100px auto",
            marginLeft: "250px",
            position: "fixed",
            width: "100%",
            padding: "10px",
          }}
        >
          <div
            style={{
              width: "100%",
              marginBottom: "25px",
              display: "flex",
              flexDirection: "row",
              alignContent: "flex-start",
              alignItems: "center",
            }}
          >
            <FolderSharedOutlinedIcon sx={{ marginRight: "10px" }} />
            <Typography>{foldername}</Typography>
            {perm === 1 ? (
              <LockIcon
                sx={{
                  fontSize: "14px",
                  color: "#000",
                  marginLeft: "7px",
                }}
              />
            ) : null}
            {data.isSuperUser ? (
              <Button
                variant="contained"
                onClick={handleClick}
                sx={{
                  mt: 0.01,
                  backgroundColor: "#212234",
                  textTransform: "capitalize",
                  marginLeft: "7px",
                  height: "20px",
                }}
                size="small"
                type="submit"
              >
                Admin
              </Button>
            ) : null}
            <Popover
              id={id}
              open={open}
              anchorEl={anchorEl}
              onClose={handleClose}
              anchorOrigin={{
                vertical: "bottom",
                horizontal: "left",
              }}
              sx={{ p: 5 }}
            >
              <div>
                <Divider />
              </div>

              <div>
                <Typography
                  sx={{
                    fontSize: "13px",
                    pl: 2,
                    fontWeight: 500,
                    "&:hover": {
                      color: "#000",
                    },
                  }}
                  className="popoverContent"
                  onClick={ManageSettings}
                >
                  Manage Team Folder
                </Typography>
                {ManageFolderSettings}
              </div>

              <div>
                <Divider />
              </div>

              <div>
                <Typography
                  sx={{ fontSize: "13px", pl: 2, fontWeight: 500 }}
                  className="popoverContent"
                  //onClick={() => handleSpreadsheetClick}
                >
                  Members
                </Typography>
              </div>

              <div>
                <Typography
                  sx={{
                    fontSize: "13px",
                    pl: 2,
                    fontWeight: 500,
                    "&:hover": {
                      color: "#000",
                    },
                  }}
                  className="popoverContent"
                  //onClick={() => handlePresentationClick}
                >
                  Settings
                </Typography>
              </div>

              <div>
                <Typography
                  sx={{ fontSize: "13px", pl: 2, fontWeight: 500 }}
                  className="popoverContent"
                  //onClick={() => handleFileUploadClick}
                >
                  Trash
                </Typography>
              </div>

              <div>
                <Divider />
              </div>

              <div>
                <Typography
                  sx={{ fontSize: "13px", pl: 2, fontWeight: 500 }}
                  className="popoverContent"
                  //onClick={() => handleFolderUploadClick}
                >
                  Leave Team Fodler
                </Typography>
              </div>
            </Popover>
          </div>

          <Files rows={files} foldername={foldername} />
        </div>
      )}
    </div>
  );

};

const Files = ({ rows, foldername }) => {
  const token = sessionStorage.getItem("token");
  const js = JSON.parse(token);

  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };
  return (
    <div className="viewCategoryDataTable">
      <div style={{ width: "83%" }}>
        <style>
          {`
                        .MuiDataGrid-columnHeader {
                          background-color: #ffffff;
                          color: #000000;
                          width: 100%;
                        }
                      `}
        </style>
        <DataGrid rows={rows} columns={fileColumns} pageSize={4} />
      </div>
    </div>
  );
};

export default ViewTeamFiles;
