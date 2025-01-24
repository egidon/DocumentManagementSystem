import React, { useState, useEffect } from "react";
import Tab from "@mui/material/Tab";
import TabPanel from "@mui/lab/TabPanel";
import TabContext from "@mui/lab/TabContext";
import TabList from "@mui/lab/TabList";
import Modal from "@mui/material/Modal";
import { TextareaAutosize } from "@mui/base/TextareaAutosize";
import { Form, useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  InputBase,
  IconButton,
  Card,
} from "@mui/material";
import { getUserAccounts, createUserAccount } from "../../config";
import moment from "moment";
import SearchIcon from "@mui/icons-material/Search";
import { styled, alpha } from "@mui/material/styles";
import { DataGrid } from "@mui/x-data-grid";
import Tooltip from "@mui/material/Tooltip";
import ModeEditOutlineOutlinedIcon from "@mui/icons-material/ModeEditOutlineOutlined";
import DeleteOutlineOutlinedIcon from "@mui/icons-material/DeleteOutlineOutlined";
import Drawer from "@mui/material/Drawer";
import FormControl from "@mui/material/FormControl";
import TextField from "@mui/material/TextField";
import InputLabel from "@mui/material/InputLabel";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";
import CardContent from "@mui/material/CardContent";
import SupervisorAccountIcon from "@mui/icons-material/SupervisorAccount";
import { getRoles, createUserRole } from "../../config";
import { Stack } from "@mui/material";
import dayjs, { Dayjs } from "dayjs";
import { DemoContainer } from "@mui/x-date-pickers/internals/demo";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";

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
  width: 600,
  bgcolor: "background.paper",
  border: "2px solid #000",
  boxShadow: 24,
  p: 4,
};

const ShowUserAccounts = ({ ua }) => {
  const [open, setOpen] = useState(false);
  const [user, setUser] = useState({
    role_id: "",
    name_of_user: "",
    empId: "",
  });
  const [roles, setRoles] = useState([]);
  const handleOpen = async (e) => {
    setOpen(true);
    setUser({
      ...user,
      role_id: e.row.id,
      name_of_user: e.row.displayName,
      empId: e.row.empId,
    });
  };
  const handleClose = () => setOpen(false);
  const [values, setValues] = useState({
    userRole: "",
    effectiveDate: dayjs(),
    expiryDate: dayjs(),
  });
  const navigate = useNavigate();
  //** FUNCTION TO LOAD ALL ROLES ON PAGE LOAD */
  useEffect(() => {
    Get_all_roles();
  }, []);

  if (!ua && ua.length === 0) {
    return null; // Return null if the data array is empty
  }
  const token = sessionStorage.getItem("token");
  const js = JSON.parse(token);
  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };

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

  const handleMenuItemClick = () => {
    navigate("/viewroles");
  };

  const handleDateChange = (name, date) => {
    setValues((prevValues) => ({
      ...prevValues,
      [name]: date,
    }));
  };
  const handleSubmit = async (e) => {
    try {
      e.preventDefault();
      const headers = new Headers({ "Content-Type": "application/json" });
      const send_values = {
        RoleID: user.role_id,
        EffectiveDate: values.effectiveDate,
        ExpiryDate: values.expiryDate,
        Status: 1,
        UserID: user.empId,
        CreatedByUserID: data.datacreatedbyuserid,
        IsOwner: false,
        token: data.datatoken,
        secretKey: data.datasecretkey,
      };
      const response = await fetch(createUserRole, {
        method: "POST",
        headers: headers,
        body: JSON.stringify(send_values),
      });
      if (response.ok) {
        const data = await response.json();
        alert(data.message);
        handleClose();
      }
    } catch (error) {}
  };

  const ManageUserRole = (
    <div>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <LocalizationProvider dateAdapter={AdapterDayjs}>
          <Box sx={style} noValidate autoComplete="off">
            <Typography id="modal-modal-title" variant="h6" component="h2">
              Manage Role for User: {user.name_of_user}
            </Typography>
            <Card>
              <CardContent sx={{ mt: 2, p: 2, m: 1 }}>
                <form>
                  <div>
                    <Box>
                      <Box
                        sx={{
                          margin: "10px",
                          marginBottom: "20px",
                          fontWeight: 500,
                          fontSize: "20px",
                        }}
                      ></Box>
                      <FormControl fullWidth>
                        <InputLabel id="demo-simple-select-label">
                          User Role
                        </InputLabel>
                        <Select
                          labelId="demo-simple-select-label"
                          id="demo-simple-select"
                          value={values.userRole}
                          label="Alias"
                          onChange={(e) => {
                            setValues({ ...values, userRole: e.target.value });
                          }}
                        >
                          {roles.length > 0 ? (
                            roles.map((item, index) => (
                              <MenuItem key={item.roleName} value={item.roleID}>
                                {item.roleName}
                              </MenuItem>
                            ))
                          ) : (
                            <MenuItem value={1} onClick={handleMenuItemClick}>
                              <Typography>Role</Typography>
                            </MenuItem>
                          )}
                        </Select>
                      </FormControl>
                    </Box>
                    {/* Expiry Date and Effective Date */}
                    <div style={{ display: "flex", flexDirection: "row" }}>
                      <Box sx={{ mt: 3, display: "flex", mr: "20px" }}>
                        <Stack spacing={4} sx={{ width: "232px" }}>
                          <DemoContainer components={["DatePicker"]}>
                            <FormControl fullWidth>
                              <DatePicker
                                label="Effective Date"
                                defaultValue={values.effectiveDate}
                                value={values.effectiveDate}
                                onChange={(date) =>
                                  handleDateChange("effectiveDate", date)
                                }
                                // slots={{ day: CustomDay }}
                              />
                            </FormControl>
                          </DemoContainer>
                        </Stack>
                      </Box>

                      <Box sx={{ mt: 3, display: "flex", mr: "20px" }}>
                        <Stack spacing={4} sx={{ width: "232px" }}>
                          <DemoContainer components={["DatePicker"]}>
                            <FormControl fullWidth>
                              <DatePicker
                                label="Expiry Date"
                                defaultValue={values.expiryDate}
                                value={values.expiryDate}
                                onChange={(date) =>
                                  handleDateChange("expiryDate", date)
                                }
                                // renderInput={(params) => (
                                //   <TextField {...params} />
                                // )}
                              />
                            </FormControl>
                          </DemoContainer>
                        </Stack>
                      </Box>
                    </div>

                    <Box>
                      <Box
                        sx={{
                          margin: "10px",
                          marginBottom: "20px",
                          fontWeight: 500,
                          fontSize: "20px",
                        }}
                      ></Box>
                      <Button
                        onClick={handleSubmit}
                        variant="contained"
                        sx={{
                          mt: 1,
                          width: "100%",
                          backgroundColor: "#212334",
                        }}
                      >
                        Add Role to User
                      </Button>
                    </Box>
                  </div>
                </form>
              </CardContent>
            </Card>
          </Box>
        </LocalizationProvider>
      </Modal>
    </div>
  );
  const columns = [
    { field: "id", headerName: "", flex: 0.1 },
    { field: "username", headerName: "Username", flex: 1 },
    { field: "displayName", headerName: "Display Name", flex: 2 },
    { field: "empId", headerName: "Employee Id", flex: 1 },
    { field: "email", headerName: "Employee Email", flex: 2 },
    {
      field: "createdOnDate",
      headerName: "Created Date",
      flex: 2,
      valueGetter: (params) => {
        // Assuming createdOnDate is a string containing the date
        const formattedDate = moment(params.createdOnDate).format(
          "MMMM Do YYYY, h:mm:ss a"
        );
        return formattedDate;
      },
    },
    {
      field: "action",
      headerName: "",
      flex: 1.4,
      renderCell: (params) => {
        return params && params.row ? (
          <div>
            <div
              style={{
                display: "flex",
                flexDirection: "row",
                justifyContent: "space-between",
              }}
            >
              <div style={{ flex: 1, padding: "5px" }}>
                <Tooltip
                  title="Edit"
                  // onClick={toggleDrawer("right", true, params)}
                >
                  <IconButton>
                    <ModeEditOutlineOutlinedIcon />
                  </IconButton>
                </Tooltip>
                <Drawer
                  anchor="right"
                  // open={state["right"]}
                  // onClose={toggleDrawer("right", false, params)}
                >
                  {/* {EditDepartment} */}
                </Drawer>
              </div>
              <div style={{ flex: 1, padding: "5px" }}>
                <Tooltip
                  title="Delete"
                  // onClick={(event) => {
                  //   handleDelete(event, params);
                  // }}
                  // onClick={(event) => handleDelete(event, params)}
                >
                  <IconButton>
                    <DeleteOutlineOutlinedIcon />
                  </IconButton>
                </Tooltip>
              </div>
              <div style={{ flex: 1, padding: "5px" }}>
                <Tooltip
                  title="Manage Roles"
                  // onClick={(event) => {
                  //   handleDelete(event, params);
                  // }}
                  onClick={() => handleOpen(params)}
                >
                  <IconButton>
                    <SupervisorAccountIcon />
                  </IconButton>
                </Tooltip>
                {ManageUserRole}
              </div>
            </div>
          </div>
        ) : null;
      },
    },
  ];

  return (
    <div className="viewCategoryDataTable">
      <div style={{ height: 400, width: "100%" }}>
        <style>
          {`
                  .MuiDataGrid-columnHeader {
                    background-color: #212334;
                    color: white;
                  }
                `}
        </style>
        <DataGrid rows={ua} columns={columns} pageSize={4} />
      </div>
    </div>
  );
};
//create new user and add to the system

const UserAccountView = () => {
  const [open, setOpen] = useState(false);
  const [useraccounts, setUserAccounts] = useState([]);
  const navigate = useNavigate();
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);
  const [values, setValues] = useState({
    username: "",
    firstname: "",
    lastname: "",
    email: "",
    password: "",
    empid: "",
    displayname: "",
  });
  const token = sessionStorage.getItem("token");
  const js = JSON.parse(token);
  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };

  //****** Get all data to be sent to server and store in an object ********
  const send_values = {
    Token: data.datatoken,
    SecretKey: data.datasecretkey,
    CreatedByUserID: data.datacreatedbyuserid,
  };

  const get_portal_users = async () => {
    if (!data.isSuperUser) {
      alert("You are not authorized to perform this operation");
      navigate("/login");
    }
    try {
      const response = await fetch(getUserAccounts, {
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
          setUserAccounts(data); // Ensure data is an array
        } else {
          alert("API response is not an array:", data);
          return;
        }
      } else {
        alert("Failed to fetch users:", response.statusText);
        return;
      }
    } catch (err) {
      alert(`error retrieving data ${err}`);
      return;
    }
  };

  //*** useEffect is to run 3 functions to get all permission keys, module def and all values in module definition */
  useEffect(() => {
    get_portal_users();
  }, []);

  const validateEmail = (email) => {
    const domain = "@nigcomsat.gov.ng";
    if (email.endsWith(domain)) {
      return true;
    } else {
      alert(`Email must end with "${domain}"`);
      return;
    }
  };

  const handleSubmit = async (e) => {
    if (!data.isSuperUser) {
      alert("You are not authorized to perform this operation");
      navigate("/login");
    }
    try {
      e.preventDefault();
      if (!values.username || values.username.trim() === "") {
        alert(`Please enter a username`);
        return;
      }
      if (!values.firstname || values.firstname.trim() === "") {
        alert(`Please enter your first name`);
        return;
      }
      if (!values.lastname || values.lastname.trim() === "") {
        alert(`Please enter your last name`);
        return;
      }
      if (!values.email || values.email.trim() === "") {
        alert(`Please enter your official email address`);
        return;
      }
      if (validateEmail(values.email)) {
        const headers = new Headers({ "Content-Type": "application/json" });
        const send_value = {
          UserName: values.username,
          FirstName: values.firstname,
          LastName: values.lastname,
          Password: values.password,
          EmpId: values.empid,
          Email: values.email,
          DisplayName: `${values.firstname} ${values.lastname}`,
          CreatedByUserID: data.datacreatedbyuserid,
          Token: data.datatoken,
          SecretKey: data.datasecretkey,
        };
        const response = await fetch(createUserAccount, {
          method: "POST",
          headers: headers,
          body: JSON.stringify(send_value),
        });
        if (response.statusText === "OK") {
          alert(`${values.displayname} created successfully`);
          setValues({
            username: "",
            firstname: "",
            lastname: "",
            email: "",
            password: "",
            empid: "",
          });
          handleClose();
          get_portal_users();
        } else {
          alert("Failed creating user: " + response.statusText);
        }
      }
      else{
        alert('all email must end with @nigcomsat.gov.ng')
        return;
      }
    } catch (error) {
      alert(`Failed Creating ${values.displayname}`);
    }
  };

  const AddNewUser = (
    <div>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style} noValidate autoComplete="off">
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Add New User
          </Typography>
          <Card>
            <CardContent sx={{ mt: 2, p: 2, m: 1 }}>
              <form>
                <div>
                  <Box>
                    <Box
                      sx={{
                        margin: "10px",
                        marginBottom: "20px",
                        fontWeight: 500,
                        fontSize: "15px",
                      }}
                    ></Box>
                    <FormControl fullWidth>
                      <TextField
                        id="outlined-multiline-flexible"
                        label="Username"
                        placeholder="Enter your username here"
                        onChange={(e) => {
                          setValues({
                            ...values,
                            username: e.target.value,
                          });
                        }}
                      />
                    </FormControl>
                  </Box>

                  <Box>
                    <Box
                      sx={{
                        margin: "10px",
                        marginBottom: "20px",
                        fontWeight: 500,
                        fontSize: "15px",
                      }}
                    ></Box>
                    <FormControl fullWidth>
                      <TextField
                        id="outlined-multiline-flexible"
                        label="Firstname"
                        placeholder="Enter your first name"
                        onChange={(e) => {
                          setValues({
                            ...values,
                            firstname: e.target.value,
                          });
                        }}
                      />
                    </FormControl>
                  </Box>

                  <Box>
                    <Box
                      sx={{
                        margin: "10px",
                        marginBottom: "20px",
                        fontWeight: 500,
                        fontSize: "15px",
                      }}
                    ></Box>
                    <FormControl fullWidth>
                      <TextField
                        id="outlined-multiline-flexible"
                        label="Lastname"
                        placeholder="Enter your last name"
                        onChange={(e) => {
                          setValues({
                            ...values,
                            lastname: e.target.value,
                          });
                        }}
                      />
                    </FormControl>
                  </Box>

                  <Box>
                    <Box
                      sx={{
                        margin: "10px",
                        marginBottom: "20px",
                        fontWeight: 500,
                        fontSize: "15px",
                      }}
                    ></Box>
                    <FormControl fullWidth>
                      <TextField
                        id="outlined-multiline-flexible"
                        label="Email Address"
                        type="email"
                        placeholder="Enter your email address"
                        onChange={(e) => {
                          setValues({
                            ...values,
                            email: e.target.value,
                          });
                        }}
                      />
                    </FormControl>
                  </Box>

                  <Box>
                    <Box
                      sx={{
                        margin: "10px",
                        marginBottom: "20px",
                        fontWeight: 500,
                        fontSize: "15px",
                      }}
                    ></Box>
                    <FormControl fullWidth>
                      <TextField
                        id="outlined-password-input"
                        label="Password"
                        type="password"
                        autoComplete="current-password"
                        onChange={(e) => {
                          setValues({
                            ...values,
                            password: e.target.value,
                          });
                        }}
                      />
                    </FormControl>
                  </Box>

                  <Box>
                    <Box
                      sx={{
                        margin: "10px",
                        marginBottom: "20px",
                        fontWeight: 500,
                        fontSize: "15px",
                      }}
                    ></Box>
                    <FormControl fullWidth>
                      <TextField
                        id="outlined-multiline-flexible"
                        label="Employee Id"
                        type="number"
                        placeholder="Enter Employee Id (numbers only. e.g 159)"
                        onChange={(e) => {
                          setValues({
                            ...values,
                            empid: e.target.value,
                          });
                        }}
                      />
                    </FormControl>
                  </Box>

                  <Box>
                    <Box
                      sx={{
                        margin: "10px",
                        marginBottom: "20px",
                        fontWeight: 500,
                        fontSize: "20px",
                      }}
                    ></Box>
                    <Button
                      onClick={handleSubmit}
                      variant="contained"
                      sx={{ mt: 1, width: "100%", backgroundColor: "#212334" }}
                    >
                      Add New User
                    </Button>
                  </Box>
                </div>
              </form>
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
        {/* <div>
          <Search
            style={{
              borderRadius: "10px",
              height: "30px",
              marginLeft: "2px",
              width: "500px",
            }}
          >
            <SearchIconWrapper>
              <SearchIcon />
            </SearchIconWrapper>
            <StyledInputBase
              placeholder="Search across your channel"
              inputProps={{ "aria-label": "search" }}
            />
          </Search>
        </div> */}

        <div style={{ display: "flex", marginRight: "30px" }}>
          <Button
            onClick={handleOpen}
            variant="contained"
            style={{ backgroundColor: "#212334", color: "fff" }}
          >
            +Add New User
          </Button>
          {AddNewUser}
        </div>
      </Box>

      <Box display="flex" alignItems="center">
        <Card
          sx={{
            width: "100%",
          }}
        >
          <ShowUserAccounts ua={useraccounts} />
        </Card>
      </Box>
    </div>
  );
};

export default UserAccountView;
