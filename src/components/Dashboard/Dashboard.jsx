import React, { useState, useEffect } from "react";
import "./Dashboard.css";
import SearchIcon from "@mui/icons-material/Search";
import NotificationsNoneIcon from "@mui/icons-material/NotificationsNone";
import { Box, Typography } from "@mui/material";
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
import FileUploadIcon from "@mui/icons-material/FileUpload";
import FolderCopyIcon from "@mui/icons-material/FolderCopy";
import FormControl from "@mui/material/FormControl";
import TextField from "@mui/material/TextField";
import CardContent from "@mui/material/CardContent";
import { createFolder, getFolders, getFiles, createFile } from "../../config";
import moment from "moment";
import { DataGrid } from "@mui/x-data-grid";
import { useNavigate } from "react-router-dom";
import { TextareaAutosize } from "@mui/base/TextareaAutosize";
import CloudUploadIcon from "@mui/icons-material/CloudUpload";
import { styled, alpha } from "@mui/material/styles";
import Select, { SelectChangeEvent } from "@mui/material/Select";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import spinner from "../../images/spinner.gif";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 400,
  bgcolor: "background.paper",
  border: "2px solid #000",
  boxShadow: 24,
  p: 4,
};

/******* Styling of the Upload Button **************** */
const VisuallyHiddenInput = styled("input")({
  clip: "rect(0 0 0 0)",
  clipPath: "inset(50%)",
  height: 1,
  overflow: "hidden",
  position: "absolute",
  bottom: 0,
  left: 0,
  whiteSpace: "nowrap",
  width: 1,
});
//** handle folder columns display */
const foldercolumns = [
  { field: "id", headerName: "", flex: 0.1 },
  {
    field: "folderName",
    headerName: "Name",
    flex: 2,
    renderCell: (params) => (
      <Typography className="popoverContent">
        <Folder className="icon" sx={{ color: "#f5a500", mr: 3 }} />
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
];

const Folders = ({ rows }) => {
  const navigate = useNavigate();
  //** Get session variable */
  const token = sessionStorage.getItem("token");
  if (token == null || token === "") {
    navigate("/login");
  }
  const js = JSON.parse(token);

  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };

  const fetchFiles = async (folderId) => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        FolderId: folderId,
        CreatedByUserId: data.datacreatedbyuserid,
      };

      const response = await fetch(getFiles, {
        // connect to api and send data to server.
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(send_values),
      });

      if (response.statusText === "OK") {
        const data = await response.json();

        if (Array.isArray(data)) {
          return data; // Ensure data is an array
        } else {
          alert("API response is not an array:", data);
          return;
        }
      } else {
        alert("You have no files in this folder :", response.statusText);
        return [];
      }
    } catch (err) {
      alert(`error retrieving data ${err}`);
      return;
    }
  };

  //** handle RowDoubleClick on folder to view all files in folder */
  const handleRowDoubleClick = async (params) => {
    const folderId = params.row.id;
    const foldername = params.row.folderName;

    navigate(`/viewfiles`, {
      state: { folderId, foldername },
    });

  };

  return (
    <div className="viewCategoryDataTable">
      <div style={{ height: 450, width: "100%" }}>
        <style>
          {`
                  .MuiDataGrid-columnHeader {
                    background-color: #ffffff;
                    color: #000000;
                    width: 100%;
                  }
                `}
        </style>
        <DataGrid
          rows={rows}
          columns={foldercolumns}
          pageSize={3}
          onRowDoubleClick={handleRowDoubleClick}
        />
      </div>
    </div>
  );
};

const Files = () => {
  return (
    <div>
      <Typography>Your file is empty</Typography>
    </div>
  );
};

const Dashboard = () => {
  const [value, setValue] = React.useState("Folders");
  const [openmodal, setOpenModal] = React.useState(false);
  const [openmodaldoc, setOpenModalDoc] = React.useState(false);
  const [file, setFile] = useState(null);
  const [showImg, setShowImg] = useState(true);

  const [opendocmodal, setOpenDocModal] = React.useState(false);
  const [folders, setFolders] = useState([]);
  const [values, setValues] = useState({
    FolderName: "",
    Description: "",
  });
  const [docs, setDocs] = useState({
    FolderName: "",
    FolderId: "",
    Description: "",
  });
  const handleChange = (event, newValue) => {
    setValue(newValue);
  };
  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };
  const navigate = useNavigate();

  //** Get session variable */
  const token = sessionStorage.getItem("token");

  // if (token === null || token === "") {
  //   window.location.href = "./login";
  // }
  const js = JSON.parse(token);

  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };

  //** Load all folders on page load and after insert */
  const Get_all_folders = async () => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        CreatedByUserId: data.datacreatedbyuserid,
      };

      const response = await fetch(getFolders, {
        // connect to api and send data to server.
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(send_values),
      });
      
      if (response.statusText === "OK") {
        const data = await response.json();

        if (Array.isArray(data)) {
          setFolders(data); // Ensure data is an array
        } else {
          alert("API response is not an array:", data);
          return;
        }
      } else {
        alert("Failed to fetch Folders:", response.statusText);
        return;
      }
    } catch (err) {
      alert(`error retrieving data ${err}`);
      return;
    }
  };

  // useEffect(() => {
  //   Get_all_folders();
  // }, []);

  useEffect(() => {
    const interval = setInterval(() => {
      Get_all_folders();
      setShowImg(false);
    }, 2000); // Run every 5000 milliseconds (5 seconds)

    // Cleanup the interval on component unmount
    return () => clearInterval(interval);
  }, []);

  //****** Popover function to display more options when +New button is clicked
  const [anchorEl, setAnchorEl] = useState(null);

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);
  const id = open ? "simple-popover" : undefined;
  //***** */ function to handle open and closing of Modals ***********
  const handleOpenModal = () => setOpenModal(true);
  const handleCloseModal = () => setOpenModal(false);

  //***** */ function to handle open and closing of Modals ***********
  const handleOpenModalDoc = () => setOpenModalDoc(true);
  const handleCloseModalDoc = () => setOpenModalDoc(false);

  const handleTypographyClick = (item) => {
    console.log(`${item} clicked`);
    handleOpenModal();
  };
  //*** All Functions of each link */
  const handleFolderClick = () => {
    handleOpenModal();
  };

  const handleDocumentClick = () => {
    handleOpenModalDoc();
  };

  const handleSpreadsheetClick = () => {
    console.log("Spreadsheet clicked");
    handleOpenModal();
  };

  const handlePresentationClick = () => {
    console.log("Presentation clicked");
    handleOpenModal();
  };

  const handleFileUploadClick = () => {
    console.log("File Upload clicked");
    handleOpenModal();
  };

  const handleFolderUploadClick = () => {
    console.log("Folder Upload clicked");
    handleOpenModal();
  };
  //** Function to Cancel a Create Folder Request */
  const handleCancel = () => {};

  //** Function to Create New Folder Request */
  const handleSubmit = async (e) => {
    e.preventDefault();
    // if (!data.isSuperUser) {
    //   alert("You are not authorized to perform this operation");
    //   return;
    // }
    try {
      e.preventDefault();
      if (!values.FolderName || values.FolderName.trim() === "") {
        alert(`Please enter a folder name`);
        return;
      }
      const headers = new Headers({ "Content-Type": "application/json" });
      const send_values = {
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        FolderName: values.FolderName,
        CreatedByUserID: data.datacreatedbyuserid,
      };
      const response = await fetch(createFolder, {
        method: "POST",
        headers: headers,
        body: JSON.stringify(send_values),
      });
      if (response.statusText === "OK") {
        alert(`${values.FolderName} created successfully`);
        setValues(null);
        handleClose();
        Get_all_folders();
      } 
    } catch (error) {
      alert(`Failed Creating ${values.FolderName}`);
    }
  };

  //** function to create new folder */
  const CreateFolder = (
    <div>
      <Modal
        open={openmodal}
        onClose={handleCloseModal}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
            New Folder
          </Typography>
          <Card>
            <CardContent sx={{ mt: 2, p: 2, m: 1 }}>
              <Box sx={{ display: "flex", mr: "20px", gap: 2 }}>
                <FormControl fullWidth>
                  <TextField
                    id="outlined-multiline-flexible"
                    label="Untitled folder"
                    onChange={(e) => {
                      setValues({ ...values, FolderName: e.target.value });
                    }}
                  />
                </FormControl>
              </Box>

              <Box sx={{ mt: 1, display: "flex", mr: "20px", gap: 2 }}>
                <Button
                  onClick={handleSubmit}
                  variant="text"
                  sx={{ mt: 1, width: "100%" }}
                >
                  Create
                </Button>

                <Button
                  onClick={handleCancel}
                  variant="text"
                  sx={{ mt: 1, width: "100%" }}
                >
                  Cancel
                </Button>
              </Box>
            </CardContent>
          </Card>
        </Box>
      </Modal>
    </div>
  );

  //** function to get all files of a particular folder */
  const Get_all_files = () => {};

  //** Function to Create New File  */
  const handleFileUpload = async (e) => {
    if (!docs.FolderId) {
      alert(`Please select a folder to upload your file`);
      return;
    }
    if (!docs.Description || docs.Description.trim() === "") {
      alert(`Please write a short description of the file`);
      return;
    }
    if (!file) {
      alert(`Please upload a file`);
      return;
    }
    const formData = new FormData();
    formData.append("Token", data.datatoken);
    formData.append("SecretKey", data.datasecretkey);
    formData.append("FolderId", docs.FolderId);
    formData.append("FolderName", docs.FolderName);
    formData.append("Description", docs.Description);
    formData.append("CreatedByUserID", data.datacreatedbyuserid);
    formData.append("FileForUpload", file);

    // Log each key-value pair in the FormData object
    // for (let pair of formData.entries()) {
    //   console.log(`${pair[0]}: ${pair[1]}`);
    // }
    try {
      e.preventDefault();
      const headers = new Headers({ "Content-Type": "application/json" });
      const response = await fetch(createFile, {
        method: "POST",
        body: formData,
      });
      if (response.statusText === "OK") {
        alert(`file uploaded successfully`);
        setDocs(null);
        handleClose();
        return;
        //Get_all_files();
      }
    } catch (error) {
      alert(`Failed Uploading file`);
    }
  };
  //** Document, Spreadsheet and Excel Upload. This function's job is to upload MS Office Files  */
  const CreateOffice = (
    <div>
      <Modal
        open={openmodaldoc}
        onClose={handleCloseModalDoc}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Add New File
          </Typography>
          <Card>
            <CardContent sx={{ mt: 2, p: 2, m: 1 }}>
              <Box sx={{ display: "flex", mr: "20px" }}>
                <Select
                  fullWidth
                  labelId="demo-simple-select-label"
                  id="demo-simple-select"
                  //value={docs.FolderId}
                  label="Alias"
                  onChange={(e) => {
                    const selectedFolder = folders.find(
                      (folder) => folder.id === e.target.value
                    );
                    setDocs({
                      ...docs,
                      FolderId: e.target.value,
                      FolderName: selectedFolder
                        ? selectedFolder.folderName
                        : "",
                    });
                  }}
                >
                  {folders.length > 0
                    ? folders.map((item, index) => (
                        <MenuItem key={index} value={item.id}>
                          {item.folderName}
                        </MenuItem>
                      ))
                    : null}
                </Select>
              </Box>

              <Box sx={{ mt: 1, display: "flex", mr: "20px" }}>
                <FormControl fullWidth>
                  <TextareaAutosize
                    minRows={3}
                    placeholder="Enter Description of Manual"
                    onChange={(e) => {
                      setDocs({ ...docs, Description: e.target.value });
                    }}
                  />
                </FormControl>
              </Box>

              <Box
                sx={{
                  mt: 1,
                  display: "flex",
                  mr: "20px",
                  flexDirection: "column",
                }}
              >
                <div>
                  {file && (
                    <Typography
                      sx={{ textWrap: "wrap" }}
                      variant="subtitle1"
                      gutterBottom
                    >
                      {file.name}
                    </Typography>
                  )}
                </div>

                <div>
                  <Button
                    sx={{
                      backgroundColor: "#373848",
                      width: "100%",
                      textTransform: "capitalize",
                    }}
                    component="label"
                    role={undefined}
                    variant="contained"
                    tabIndex={-1}
                    startIcon={<CloudUploadIcon />}
                  >
                    Select File
                    <VisuallyHiddenInput
                      type="file"
                      onChange={handleFileChange}
                      accept=".pdf, .docx, .pptx, .xlsx"
                    />
                  </Button>
                </div>
              </Box>

              <Box sx={{ mt: 1, display: "flex", mr: "20px" }}>
                <Button
                  onClick={handleFileUpload}
                  variant="contained"
                  sx={{ mt: 1, width: "100%", backgroundColor: "#212334" }}
                >
                  Upload File
                </Button>
              </Box>
            </CardContent>
          </Card>
        </Box>
      </Modal>
    </div>
  );

  return (
    <div>
      {/* <div className="firstRowdiv">
        <div className="RecentTitle">Recent</div>
        <div className="searchNotification">
          <div className="dashboardSearch">
            <SearchIcon />
          </div>
          <div className="dashboardNotifications">
            <NotificationsNoneIcon />
          </div>
        </div>
      </div>

      <div className="thirdRowdiv">
        <div className="newButton">
          <Button
            variant="contained"
            onClick={handleClick}
            sx={{
              mt: 0.01,
              backgroundColor: "#10ac84",
              textTransform: "capitalize",
            }}
            size="small"
            type="submit"
          >
            + New
          </Button>
          <Popover
            id={id}
            open={open}
            anchorEl={anchorEl}
            onClose={handleClose}
            anchorOrigin={{
              vertical: "bottom",
              horizontal: "left",
            }}
          >
            <div>
              <Typography
                className="popoverContent"
                onClick={() => handleFolderClick()}
              >
                <Folder className="icon" sx={{ color: "#f5a500" }} />
                Folder
              </Typography>
              {CreateFolder}
            </div>

            <div>
              <hr />
            </div>

            <div>
              <Typography
                className="popoverContent"
                onClick={() => handleDocumentClick()}
              >
                <ArticleIcon className="icon" sx={{ color: "#004F98" }} />
                Document
              </Typography>
              {CreateOffice}
            </div>

            <div>
              <Typography
                className="popoverContent"
                onClick={() => handleSpreadsheetClick}
              >
                <BorderAllIcon className="icon" sx={{ color: "#3cb043" }} />
                Spreadsheet
              </Typography>
            </div>

            <div>
              <Typography
                className="popoverContent"
                onClick={() => handlePresentationClick}
              >
                <CoPresentIcon className="icon" sx={{ color: "#D24726" }} />
                Presentation
              </Typography>
            </div>

            <div>
              <hr />
            </div>

            <div>
              <Typography
                className="popoverContent"
                onClick={() => handleFileUploadClick}
              >
                <FileUploadIcon className="icon" sx={{ color: "#808080" }} />
                File Upload
              </Typography>
            </div>

            <div>
              <Typography
                className="popoverContent"
                onClick={() => handleFolderUploadClick}
              >
                <FolderCopyIcon className="icon" sx={{ color: "#808080" }} />
                Folder Upload
              </Typography>
            </div>
          </Popover>
        </div>

        <div className="pipediv"></div>
        <div className="filterdiv">
          <FilterAltOutlinedIcon />
        </div>
      </div> */}
      {showImg ? (
        <img src={spinner} alt="spinner" style={{
          width:"5%",
          height:"5%",
          marginTop:"100px",
          alignContent:"center",
        }}/>
      ) : (
        <div
          className="secondRowdiv"
          style={{
            margin: "100px auto",
            marginLeft: "250px",
            position: "fixed",
            width: "90%",
            padding: "10px",
          }}
        >
          <TabContext value={value}>
            
            <div className="tabTextDisplay">
              <TabList
                onChange={handleChange}
                aria-label="lab API tabs example"
              >
                <Tab
                  style={{ fontSize: "14px", textTransform: "uppercase" }}
                  label="Folders"
                  value="Folders"
                />
                <Tab
                  style={{ fontSize: "14px", textTransform: "uppercase" }}
                  label="Files"
                  value="Files"
                />
              </TabList>
            </div>
            <TabPanel value="Folders">
              <div>
                <Folders rows={folders} />
              </div>
            </TabPanel>
            <TabPanel value="Files">
              <div>
                <Files />
              </div>
            </TabPanel>

          </TabContext>
        </div>
      )}
    </div>
  );
};

export default Dashboard;
