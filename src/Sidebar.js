import React, { useState, useEffect } from "react";
import "./Sidebar.css";
import Box from "@mui/material/Box";
import Drawer from "@mui/material/Drawer";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import CssBaseline from "@mui/material/CssBaseline";
import Toolbar from "@mui/material/Toolbar";
import List from "@mui/material/List";
import Divider from "@mui/material/Divider";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import LogoutIcon from "@mui/icons-material/Logout";
import FolderIcon from "@mui/icons-material/Folder";
import { Folder, Star, RecentActors } from "@mui/icons-material";
import PeopleAltOutlinedIcon from "@mui/icons-material/PeopleAltOutlined";
import { styled, Typography } from "@mui/material";
import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import FolderSharedOutlinedIcon from "@mui/icons-material/FolderSharedOutlined";
import Modal from "@mui/material/Modal";
import { Button, Card } from "@mui/material";
import CardContent from "@mui/material/CardContent";
import FormControl from "@mui/material/FormControl";
import TextField from "@mui/material/TextField";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import FormLabel from "@mui/material/FormLabel";
import { createTeamFolder, getTeamFolders } from "../src/config";
import LockIcon from "@mui/icons-material/Lock";
import SettingsSuggestIcon from "@mui/icons-material/SettingsSuggest";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: "80%",
  height: "80%",
  bgcolor: "background.paper",
  border: "1px solid #000",
  boxShadow: 24,
  p: 4,
};

const drawerWidth = 240;
const font_size = "10px";

const routeMappings = {
  MyFolder: "/myfolder",
  Recent: "/recent",
  Favourites: "/favourites",
  SharedWithMe: "/sharedwithme",
};

const iconMapping = {
  MyFolder: <Folder />,
  Recent: <RecentActors />,
  Favourites: <Star />,
  SharedWithMe: <PeopleAltOutlinedIcon />,
};

// const Drawer= styled(Drawer)(({theme})=>{

/**
 *
 *
 * @return {*}
 */
const Sidebar = () => {
  const [openmodal, setOpenModal] = React.useState(false);
  const [teamfolders, setTeamFolders] = useState([]);
  const [values, setValues] = useState({
    FolderName: "",
    Permission: "",
    Description: "",
  });

  //** Load all folders on page load and after insert */
  const Get_Team_folders = async () => {
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
        alert("Failed to fetch Folders:", response.statusText);
        return;
      }
    } catch (err) {
      alert(`error retrieving data ${err}`);
      return;
    }
  };

  const navigate = useNavigate();
  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (!token) {
      navigate("/login");
    } else {
      Get_Team_folders();
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

  //***** */ function to handle open and closing of Modals ***********
  const handleOpenModal = () => setOpenModal(true);
  const handleCloseModal = () => setOpenModal(false);

  const logout = (event) => {
    sessionStorage.setItem("token", "");
    // Redirect to the login page
    //window.location.href = "/login";
    event.preventDefault(); // Prevent the default action
    navigate("/login");
  };

  //logout function
  const handleLogout = (e) => {
    logout(e);
  };

  const Settings = (event) => {
    event.preventDefault(); // Prevent the default action
    navigate("/createrole");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!data.isSuperUser) {
      alert("You are not authorized to perform this operation");
      return;
    }
    try {
      if (!values.FolderName || values.FolderName.trim() === "") {
        alert(`Please enter a folder name`);
        return;
      }
      if (!values.Permission || values.Permission.trim() === "") {
        alert(`Please select the type of permission you are assigning.`);
        return;
      }
      if (!values.Description || values.Description.trim() === "") {
        alert(`Please enter a folder description.`);
        return;
      }
      const headers = new Headers({ "Content-Type": "application/json" });
      const send_values = {
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        FolderName: values.FolderName,
        Permission: values.Permission,
        CreatedByUserID: data.datacreatedbyuserid,
        Description: values.Description,
      };

      const response = await fetch(createTeamFolder, {
        method: "POST",
        headers: headers,
        body: JSON.stringify(send_values),
      });
      if (response.statusText === "OK") {
        alert(`${values.FolderName} created successfully`);
        setValues({ FolderName: "", Permission: false, Description: "" });
        handleCloseModal();
        Get_Team_folders();
      }
    } catch (error) {
      alert(`Failed Creating ${values.FolderName}`);
    }
  };

  const handleCancel = () => {
    handleCloseModal();
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
        <Box
          sx={{
            ...style,
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Create Team Folder
          </Typography>
          <Divider sx={{ background: "#000000" }} />
          <Card sx={{ width: "60%", border: 0, boxShadow: "none" }}>
            <CardContent>
              <Box sx={{ display: "flex", mr: "20px", gap: 2 }}>
                <FormControl fullWidth>
                  <TextField
                    id="outlined-multiline-flexible"
                    label="Enter a Team Folder name"
                    onChange={(e) => {
                      setValues({ ...values, FolderName: e.target.value });
                    }}
                  />
                </FormControl>
              </Box>

              <Box
                sx={{
                  display: "flex",
                  mr: "20px",
                  gap: 2,
                  mt: 2,
                  flexDirection: "row",
                }}
              >
                <FormControl>
                  <FormLabel id="demo-radio-buttons-group-label">
                    Choose your team folder type
                  </FormLabel>
                  <RadioGroup
                    aria-labelledby="demo-radio-buttons-group-label"
                    defaultValue="female"
                    name="radio-buttons-group"
                    row
                    onChange={(e) =>
                      setValues({
                        ...values,
                        Permission: e.target.value,
                      })
                    }
                  >
                    <FormControlLabel
                      value="0"
                      control={<Radio />}
                      label="public"
                    />
                    <FormControlLabel
                      value="1"
                      control={<Radio />}
                      label="private"
                    />
                  </RadioGroup>
                </FormControl>
              </Box>

              <Box
                sx={{
                  display: "flex",
                  mt: 2,
                  mr: "20px",
                  gap: 2,
                  // "& .MuiTextField-root": { m: 1, },
                }}
              >
                <FormControl fullWidth>
                  <TextField
                    id="outlined-multiline-static"
                    label="Folder Description"
                    multiline
                    maxRows={4}
                    onChange={(e) => {
                      setValues({ ...values, Description: e.target.value });
                    }}
                  />
                </FormControl>
              </Box>

              <Box
                sx={{
                  mt: 1,
                  display: "flex",
                  justifyContent: "flex-end",
                  gap: 2,
                  mr: 2,
                }}
              >
                <Button
                  onClick={handleCancel}
                  variant="text"
                  sx={{ mt: 1, width: "10%" }}
                >
                  Cancel
                </Button>

                <Button
                  onClick={handleSubmit}
                  variant="text"
                  sx={{
                    mt: 1,
                    width: "10%",
                    background: "#02b85c",
                    color: "#ffffff",
                  }}
                >
                  Create
                </Button>
              </Box>
            </CardContent>
          </Card>
        </Box>
      </Modal>
    </div>
  );

  //Create New Team
  const createTeam = () => {
    handleOpenModal();
  };

  //Get Folder name and ID to display all files in folder
  const displayTeamFiles = (folderid, folderName, permission) => {
    navigate(`/viewTeamfiles`, {
      state: { folderid, folderName, p: permission },
    });
  };


  return (
    <Box sx={{ display: "flex" }}>
      <Divider sx={{ backgroundColor: "white" }} />
      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: {
            width: drawerWidth,
            boxSizing: "border-box",
            backgroundColor: "#1A1A1A",
          },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: "auto", color: "#FFF" }}>
          <List>
            {Object.keys(routeMappings).map((text, index) => (
              <ListItem key={text} disablePadding>
                <Link
                  to={routeMappings[text]}
                  style={{ textDecoration: "none" }}
                >
                  <ListItemButton>
                    <ListItemIcon
                      style={{ fontSize: "20px", color: "#fff" }}
                      children={iconMapping[text]}
                    />
                    <ListItemText
                      primary={text}
                      //sx={{ fontSize: "12px", color: "#fff" }}
                      style={{ fontSize: "10px", color: "#fff" }}
                    />
                  </ListItemButton>
                </Link>
              </ListItem>
            ))}
          </List>
        </Box>
        <Divider sx={{ backgroundColor: "white" }} />

        {data.isSuperUser ? (
          <>
            <Box>
              <Link
                to="/showallfolders"
                style={{ textDecoration: "none", color: "#fff" }}
              >
                <ListItemButton>
                  <ListItemIcon style={{ fontSize: "20px", color: "#fff" }}>
                    <FolderIcon />
                  </ListItemIcon>
                  <ListItemText
                    primary="All Folders"
                    sx={{ fontSize: "12px", color: "#fff" }}
                  />
                </ListItemButton>
              </Link>
            </Box>
            <Divider sx={{ backgroundColor: "white" }} />
          </>
        ) : null}

        {/* This section will display all team folders  that has permissions, roles and
        users in them */}

        <>
          <Box
            sx={{
              mt: 2,
              overflow: "auto",
              color: "#FFF",
              fontSize: "15px",
              p: 2,
              display: "flex",
            }}
          >
            <div style={{ display: "flex", flexDirection: "column" }}>
              <div
                style={{
                  display: "flex",
                  flexDirection: "row",
                  justifyContent: "space-between",
                  alignItems: "center",
                  width: "100%",
                  gap: 17,
                }}
              >
                <Typography>TEAM FOLDERS(15)</Typography>
                {data.isSuperUser ? (
                  <AddCircleOutlineIcon
                    onClick={createTeam}
                    sx={{ marginLeft: "auto", cursor: "pointer" }}
                  />
                ) : null}
                {CreateFolder}
              </div>

              <div
                style={{
                  background: "#1a1a1a",
                  width: "100%",
                  height: "auto",
                  maxHeight: "50vh",
                }}
              >
                <List>
                  {teamfolders && teamfolders.length > 0
                    ? teamfolders.map((val, index) => (
                        <ListItem disablePadding key={index}>
                          <ListItemButton
                            onClick={() =>
                              displayTeamFiles(
                                val.id,
                                val.folderName,
                                val.permission
                              )
                            }
                          >
                            <ListItemIcon
                              style={{ fontSize: "20px", color: "#fff" }}
                            >
                              <FolderSharedOutlinedIcon
                                sx={{ marginLeft: "-18px" }}
                              />
                            </ListItemIcon>
                            <ListItemText
                              primary={val.folderName}
                              style={{
                                fontSize: "8px",
                                color: "#fff",
                                marginLeft: "-14px",
                              }}
                            />
                            {val.permission === "1" ? (
                              <LockIcon
                                sx={{
                                  fontSize: "14px",
                                  color: "#ffffff",
                                  marginLeft: "15px",
                                }}
                              />
                            ) : null}
                          </ListItemButton>
                        </ListItem>
                      ))
                    : null}
                </List>
              </div>
            </div>
          </Box>
          <Divider />
        </>

        {/* Settings */}
        {data.isSuperUser ? (
          <Box>
            <Link
              onClick={Settings}
              style={{ textDecoration: "none", color: "#fff" }}
            >
              <ListItemButton>
                <ListItemIcon style={{ fontSize: "20px", color: "#fff" }}>
                  <SettingsSuggestIcon />
                </ListItemIcon>
                <ListItemText
                  primary="Settings"
                  sx={{ fontSize: "12px", color: "#fff" }}
                />
              </ListItemButton>
            </Link>
          </Box>
        ) : null}

        {/* Logout */}
        <Box>
          <Link
            onClick={handleLogout}
            style={{ textDecoration: "none", color: "#fff" }}
          >
            <ListItemButton>
              <ListItemIcon style={{ fontSize: "20px", color: "#fff" }}>
                <LogoutIcon />
              </ListItemIcon>
              <ListItemText
                primary="Logout"
                sx={{ fontSize: "12px", color: "#fff" }}
              />
            </ListItemButton>
          </Link>
        </Box>
      </Drawer>
    </Box>
  );
};

export default Sidebar;
