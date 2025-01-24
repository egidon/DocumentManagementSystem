import React, { useState, useEffect } from "react";
import "./TopBar.css";
import SearchIcon from "@mui/icons-material/Search";
import NotificationsNoneIcon from "@mui/icons-material/NotificationsNone";
import { Box, CssBaseline, Typography } from "@mui/material";
import { Button, Card } from "@mui/material";
import { Folder } from "@mui/icons-material";
import Modal from "@mui/material/Modal";
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
import { useNavigate } from "react-router-dom";
import { TextareaAutosize } from "@mui/base/TextareaAutosize";
import CloudUploadIcon from "@mui/icons-material/CloudUpload";
import Divider from "@mui/material/Divider";
import { styled, alpha } from "@mui/material/styles";
import Select, { SelectChangeEvent } from "@mui/material/Select";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import {
  createFolder,
  getFolders,
  getFiles,
  createFile,
  createTeamFile,
  getTeamFolders,
} from "../../config";

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

const TopBar = () => {
  const [openmodal, setOpenModal] = React.useState(false);
  const [openteammodal, setTeamOpenModal] = React.useState(false);
  const [openmodaldoc, setOpenModalDoc] = React.useState(false);
  const [openteammodaldoc, setTeamOpenModalDoc] = React.useState(false);
  const [values, setValues] = useState({
    FolderName: "",
    Description: "",
  });
  const [folders, setFolders] = useState([]);
  const [teamfolders, setTeamFolders] = useState([]);
  const [file, setFile] = useState(null);
  const [teamfile, setTeamFile] = useState(null);
  const [docs, setDocs] = useState({
    FolderName: "",
    FolderId: "",
    Description: "",
  });
  const [teamdocs, setTeamDocs] = useState({
    FolderName: "",
    FolderId: "",
    Description: "",
  });

  //** Get session variable */
  const navigate = useNavigate();
  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (!token) {
      navigate("/login");
    } else {
      Get_all_folders();
      Get_all_team_folders();
    }
  }, [navigate]);

  const token = sessionStorage.getItem("token");
  let data;
  if (token) {
    try {
      const js = JSON.parse(token);
      data = {
        isSuperUser: js[0].isSuperUser,
        datatoken: js[0].token,
        datasecretkey: js[0].secretKey,
        datacreatedbyuserid: js[0].empId,
      };
    } catch (error) {
      console.error("Error parsing token:", error);
      navigate("/login");
    }
  } else {
    data = {
      isSuperUser: false,
    };
  }

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  const handleTeamFileChange = (e) => {
    setTeamFile(e.target.files[0]);
  };
  //****** Popover function to display more options when +New button is clicked
  const [anchorEl, setAnchorEl] = useState(null);

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  //****** Popover function to display more options when +New Team button is clicked
  const [anchorteamEl, setAnchorTeamEl] = useState(null);

  const handleTeamClick = (event) => {
    setAnchorTeamEl(event.currentTarget);
  };

  const handleTeamClose = () => {
    setAnchorTeamEl(null);
  };

  const teamopen = Boolean(anchorteamEl);
  const teamid = teamopen ? "simple-popover" : undefined;

  //***** */ function to handle open and closing of Modals ***********
  const handleteamOpenModal = () => setTeamOpenModal(true);
  const handleteamCloseModal = () => setTeamOpenModal(false);

  //***** */ function to handle open and closing of Modals ***********
  const handleTeamOpenModalDoc = () => setTeamOpenModalDoc(true);
  const handleTeamCloseModalDoc = () => setTeamOpenModalDoc(false);

  const handleDocsClick = () => {
    handleTeamOpenModalDoc();
  };

  //** Function to Add a Team File to a Folder */
  const handleTeamFileUpload = async (e) => {
    if (!teamdocs.FolderId) {
      alert(`Please select a folder to upload your file`);
      return;
    }
    if (!teamdocs.Description || teamdocs.Description.trim() === "") {
      alert(`Please write a short description of the file`);
      return;
    }
    if (!teamfile) {
      alert(`Please upload a file`);
      return;
    }
    const formData = new FormData();
    formData.append("Token", data.datatoken);
    formData.append("SecretKey", data.datasecretkey);
    formData.append("FolderId", teamdocs.FolderId);
    formData.append("FolderName", teamdocs.FolderName);
    formData.append("Description", teamdocs.Description);
    formData.append("CreatedByUserId", data.datacreatedbyuserid);
    formData.append("FileForUpload", teamfile);
    
    formData.forEach((value, key) => {
      console.log(key + ": " + value);
  });

    try {
      e.preventDefault();
      const headers = new Headers({ "Content-Type": "application/json" });
      
      const response = await fetch(createTeamFile, {
        method: "POST",
        body: formData,
      });
      if (response.statusText === "OK") {
        alert(`file uploaded successfully`);
        setTeamDocs(null);
        handleTeamClose();
        return;
        //Get_all_files();
      }
    } catch (error) {
      alert(`Failed Uploading file`);
    }
  };

    //** Load all team folders on page load and after insert */
    const Get_all_team_folders = async () => {
      try {
        const send_values = {
          // Get all data to be sent to server and store in an object
          Token: data.datatoken,
          SecretKey: data.datasecretkey,
          CreatedByUserId: data.datacreatedbyuserid,
        };
  
        const response = await fetch(getTeamFolders, {
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
            setTeamFolders(data); // Ensure data is an array
          } else {
            alert("API response is not an array:", data);
            return;
          }
        } else {
          alert("Failed to fetch Team Folders:", response.statusText);
          return;
        }
      } catch (err) {
        alert(`error retrieving data ${err}`);
        return;
      }
    };
  //******************* END +New Button Code here */

  const open = Boolean(anchorEl);
  const id = open ? "simple-popover" : undefined;
  //***** */ function to handle open and closing of Modals ***********
  const handleOpenModal = () => setOpenModal(true);
  const handleCloseModal = () => setOpenModal(false);

  //***** */ function to handle open and closing of Modals ***********
  const handleOpenModalDoc = () => setOpenModalDoc(true);
  const handleCloseModalDoc = () => setOpenModalDoc(false);

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

  //** Document, Spreadsheet and Excel Upload.
  //** This function's job is to upload MS Office Files  */
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

  //** Document, Spreadsheet and Excel Upload. This function's job is to upload MS Office Files  */
  const CreateTeamOffice = (
    <div>
      <Modal
        open={openteammodaldoc}
        onClose={handleTeamCloseModalDoc}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
            New Team File
            <Divider />
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
                    const selectedFolder = teamfolders.find(
                      (teamfolder) => teamfolder.id === e.target.value
                    );
                    setTeamDocs({
                      ...teamdocs,
                      FolderId: e.target.value,
                      FolderName: selectedFolder
                        ? selectedFolder.folderName
                        : "",
                    });
                  }}
                >
                  {teamfolders.length > 0
                    ? teamfolders.map((item, index) => (
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
                      setTeamDocs({ ...teamdocs, Description: e.target.value });
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
                  {teamfile && (
                    <Typography
                      sx={{ textWrap: "wrap" }}
                      variant="subtitle1"
                      gutterBottom
                    >
                      {teamfile.name}
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
                      onChange={handleTeamFileChange}
                      accept=".pdf, .docx, .pptx, .xlsx"
                    />
                  </Button>
                </div>
              </Box>

              <Box sx={{ mt: 1, display: "flex", mr: "20px" }}>
                <Button
                  onClick={handleTeamFileUpload}
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
    <div
      style={{
        position: "fixed",
        zIndex: (theme) => theme.zIndex.drawer + 1,
        width: "100%",
      }}
    >
      <div className="firstRowdiv">
        <div className="RecentTitle">Document Management System</div>
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
            onClick={handleTeamClick}
            sx={{
              mt: 0.01,
              backgroundColor: "#212234",
              textTransform: "capitalize",
              marginRight: "10px",
            }}
            size="small"
            type="submit"
          >
            + New Team
          </Button>
          <Popover
            id={teamid}
            open={teamopen}
            anchorEl={anchorteamEl}
            onClose={handleTeamClose}
            anchorOrigin={{
              vertical: "bottom",
              horizontal: "left",
            }}
          >
            {/* <div>
              <Typography
                className="popoverContent"
                onClick={() => handleFolderClick()}
              >
                <Folder className="icon" sx={{ color: "#f5a500" }} />
                Team Folder
              </Typography>
              {CreateTeamFolder}
            </div> */}

            <div>
              <Divider />
            </div>

            <div>
              <Typography
                className="popoverContent"
                onClick={() => handleDocsClick()}
              >
                <ArticleIcon className="icon" sx={{ color: "#004F98" }} />
                Team Docs
              </Typography>
              {CreateTeamOffice}
            </div>

            <div>
              <Typography
                className="popoverContent"
                onClick={() => handleSpreadsheetClick}
              >
                <BorderAllIcon className="icon" sx={{ color: "#3cb043" }} />
                Team Sheets
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
              <Divider />;
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
              <Divider />
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
              <Divider />;
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
      </div>
    </div>
  );
};

export default TopBar;
