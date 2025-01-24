import React, { useState, useEffect } from "react";
import Tab from "@mui/material/Tab";
import TabPanel from "@mui/lab/TabPanel";
import TabContext from "@mui/lab/TabContext";
import TabList from "@mui/lab/TabList";
import Modal from "@mui/material/Modal";
import { TextareaAutosize } from "@mui/base/TextareaAutosize";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  InputBase,
  IconButton,
  Card,
} from "@mui/material";

import SearchIcon from "@mui/icons-material/Search";
import { styled, alpha } from "@mui/material/styles";
import { DataGrid } from "@mui/x-data-grid";
import Tooltip from "@mui/material/Tooltip";
import ModeEditOutlineOutlinedIcon from "@mui/icons-material/ModeEditOutlineOutlined";
import DeleteOutlineOutlinedIcon from "@mui/icons-material/DeleteOutlineOutlined";
import Drawer from "@mui/material/Drawer";
import FormControl from "@mui/material/FormControl";
import TextField from "@mui/material/TextField";
import CardContent from "@mui/material/CardContent";
import { createRole } from "../../config";
import { getRoles } from "../../config";
import Badge from "@mui/material/Badge";
// import CreatePermissions from "../AdminSettings/CreatePermissions";
import UserAccountView from "./UserAccountView";

const Search = styled("div")(({ theme }) => ({
  position: "relative",
  display: "flex",
  flexDirection: "row",
  borderRadius: theme.shape.borderRadius,
  backgroundColor: alpha(theme.palette.common.white, 0.15),
  "&:hover": {
    backgroundColor: alpha(theme.palette.common.white, 0.25),
  },
  marginRight: theme.spacing(2),
  marginLeft: 0,
  width: "100%",
  [theme.breakpoints.up("sm")]: {
    marginLeft: theme.spacing(3),
    width: "auto",
  },
}));

const SearchIconWrapper = styled("div")(({ theme }) => ({
  padding: theme.spacing(0, 2),
  height: "100%",
  position: "absolute",
  pointerEvents: "none",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
}));

const StyledInputBase = styled(InputBase)(({ theme }) => ({
  color: "inherit",
  "& .MuiInputBase-input": {
    padding: theme.spacing(1, 1, 1, 0),
    // vertical padding + font size from searchIcon
    paddingLeft: `calc(1em + ${theme.spacing(4)})`,
    transition: theme.transitions.create("width"),
    // width: "100%",
    width: "500px",
    [theme.breakpoints.up("md")]: {
      // width: "20ch",
      width: "50ch",
    },
  },
}));

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

const rolestyle = {
  position: "relative",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 270,
  height: 150,
  //bgcolor: "background.paper",
  bgcolor: "#1A1A29",
  p: 4,
  borderRadius: 2,
  display: "inline-block",
  margin: 1,
};
//***************** SECTION TO DISPLAY RECORDS IN THE DATABASE */
const ViewNewRole = () => {
  const [open, setOpen] = React.useState(false);
  const [roles, setRoles] = useState([]);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);
  const navigate = useNavigate();
  const [values, setValues] = useState({
    RoleName: "",
    Description: "",
  });

  const token = sessionStorage.getItem("token");
  const js = JSON.parse(token);
  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };
  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (token === null) {
      navigate("/login");
    }
  }, [navigate]);

  const Get_all_roles = async () => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        CreatedByUserID: data.datacreatedbyuserid,
      };

      const response = await fetch(getRoles, {
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
          setRoles(data); // Ensure data is an array
        } else {
          alert("API response is not an array:", data);
          return;
        }
      } else {
        alert("Failed to fetch roles:", response.statusText);
        return;
      }
    } catch (err) {
      alert(`error retrieving data ${err}`);
      return;
    }
  };

  //** FUNCTION TO LOAD ALL ROLES ON PAGE LOAD */
  useEffect(() => {
    Get_all_roles();
  }, []);

  const handleSubmit = async (e) => {
    if (!data.isSuperUser) {
      alert("You are not authorized to perform this operation");
      navigate("/login");
    }

    try {
      e.preventDefault();
      if (!values.RoleName || values.RoleName.trim() === "") {
        alert(`Please enter a role name`);
        return;
      }
      if (!values.Description || values.Description.trim() === "") {
        alert(`Please tell us a bit what this role is all about in the
            description box`);
        return;
      }
      const headers = new Headers({ "Content-Type": "application/json" });
      const send_values = {
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        RoleName: values.RoleName,
        Description: values.Description,
        CreatedByUserID: data.datacreatedbyuserid,
      };
      const response = await fetch(createRole, {
        method: "POST",
        headers: headers,
        body: JSON.stringify(send_values),
      });
      if (response.statusText === "OK") {
        alert(`${values.RoleName} created successfully`);
        setValues(null);
        handleClose();
        Get_all_roles();
      }
    } catch (error) {
      alert(`Failed Creating ${values.RoleName}`);
    }
  };

  //** FUNCTION TO ADD A NEW ROLE TO THE SYSTEM */
  const AddNewRole = (
    <div>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Add New Role
          </Typography>
          <Card>
            <CardContent sx={{ mt: 2, p: 2, m: 1 }}>
              <Box sx={{ display: "flex", mr: "20px" }}>
                <FormControl fullWidth>
                  <TextField
                    id="outlined-multiline-flexible"
                    label="Enter Role Name"
                    onChange={(e) => {
                      setValues({ ...values, RoleName: e.target.value });
                    }}
                  />
                </FormControl>
              </Box>
              <Box sx={{ mt: 1, display: "flex", mr: "20px" }}>
                <FormControl fullWidth>
                  <TextareaAutosize
                    label="Describe what the role does"
                    minRows={3}
                    onChange={(e) => {
                      setValues({ ...values, Description: e.target.value });
                    }}
                  />
                </FormControl>
              </Box>
              <Box sx={{ mt: 1, display: "flex", mr: "20px" }}>
                <Button
                  onClick={handleSubmit}
                  variant="contained"
                  sx={{ mt: 1, width: "100%", backgroundColor: "#212334" }}
                >
                  Add Role
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
        display: "flex",
        flexDirection: "column",
      }}
    >
      <Box
        display="flex"
        flexDirection="row"
        alignItems="center"
        marginBottom={1.5}
        justifyContent="flex-end"
      >
        <div style={{ display: "flex", marginRight: "30px" }}>
          <Button
            onClick={handleOpen}
            variant="contained"
            style={{ backgroundColor: "#212334", color: "fff" }}
          >
            +Add New Role
          </Button>
          {AddNewRole}
        </div>
      </Box>

      <Box display="flex" alignItems="center">
        <Card
          sx={{
            width: "100%",
          }}
        >
          {<AllRoles roles={roles} />}
        </Card>
      </Box>
    </div>
  );
};

//*********** FUNCTION TO DISPLAY ALL ROLES IN APPLICATION */
const AllRoles = ({ roles }) => {
  //const [roles, setRoles] = useState([]);
  const token = sessionStorage.getItem("token");
  const js = JSON.parse(token);
  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };
  return (
    <div style={{ backgroundColor: "#fff" }}>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(4, 1fr)",
          gap: 1,
        }}
      >
        {roles && roles.length > 0 ? (
          roles.map((value, key) => (
            <Box sx={rolestyle} key={key} style={{ margin: "5px" }}>
              <Card sx={{ margin: "auto", boxShadow: "none" }}>
                <CardContent sx={{ mt: 2, p: 2, m: 1 }}>
                  <Typography
                    sx={{
                      textAlign: "center",
                      fontWeight: "700",
                      fontSize: "16px",
                    }}
                  >
                    {value.roleName}
                  </Typography>
                  <hr />
                </CardContent>
                {value.roleName !== "Administrator" &&
                value.roleName !== "Superuser" ? (
                  <Button //onClick={handleSubmit}
                    variant="contained"
                    sx={{
                      position: "absolute",
                      fontSize: 10,
                      bottom: 20,
                      right: 160,
                      transform: "translate(50%, 50%)",
                      backgroundColor: "#212334",
                      width: 200,
                      textTransform: "capitalize",
                    }}
                  >
                    Configure Permissions
                  </Button>
                ) : null}

                <Badge
                  badgeContent={"6"}
                  color="primary"
                  sx={{
                    position: "absolute",
                    bottom: 20,
                    right: 20,
                    transform: "translate(50%, 50%)",
                  }}
                />
              </Card>
            </Box>
          ))
        ) : (
          <Box>No roles found</Box>
        )}
      </Box>
    </div>
  );
};

//** THIS IS THE MAIN FUNCTION TO DISPLAY ALL THE OTHER FUNCTIONS. */
const CreateRoles = () => {
  const [value, setValue] = React.useState("Roles");
  const handleChange = (event, newValue) => {
    setValue(newValue);
  };
  const navigate = useNavigate();
  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (token === null) {
      navigate("/login");
    }
  }, [navigate]);
  return (
    <Box
      sx={{
        margin: "80px auto",
        padding: "10px",
        marginRight: "10px",
        "@media (min-width: 768px)": {
          marginLeft: "250px",
        },
      }}
    >
      <Box component="main" sx={{ flexGrow: 1, p: 1.5 }}>
        <Box
          sx={{
            display: "flex",
            marginBottom: "20px",
            alignItems: "center",
            textAlign: "center",
            marginLeft: "0px",
            // backgroundColor: "#e5e7ee",
            // borderBottomRightRadius: "13px",
            // borderBottomLeftRadius: "13px",
            width: "100%",
          }}
        >
          <TabContext value={value}>
            <div
              sx={{
                borderBottom: 1,
                display: "flex",
                alignItems: "left",
                textAlign: "left",
                marginLeft: "0px",
              }}
            >
              <TabList
                onChange={handleChange}
                aria-label="lab API tabs example"
                textTransform="uppercase"
              >
                <Tab
                  style={{
                    fontSize: "14px",
                  }}
                  label="Roles"
                  value="Roles"
                />

                <Tab
                  style={{
                    fontSize: "14px",
                  }}
                  label="User Accounts"
                  value="UserAccounts"
                />
              </TabList>
            </div>
          </TabContext>
        </Box>
        <TabContext value={value}>
          <TabPanel value="Roles">
            <ViewNewRole />
          </TabPanel>
          <TabPanel value="UserAccounts">
            <UserAccountView />
          </TabPanel>
        </TabContext>
      </Box>
    </Box>
  );
};

export default CreateRoles;
