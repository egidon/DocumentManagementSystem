import React, { useEffect, useState } from "react";
import moment from "moment";
import { Box, Typography, Link, Button } from "@mui/material";
import {
  getRoles,
  getTeamFolderMembers,
  getUserAccounts,
  addMemberToFolder,
} from "../../config.js";
import Chip from "@mui/material/Chip";
import Autocomplete from "@mui/material/Autocomplete";
import TextField from "@mui/material/TextField";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";

const Members = ({ folderid }) => {
  //const [folders, setFolders] = useState([]);
  const [members, setMembers] = useState([]);
  const [getroles, setGetRoles] = useState([]);
  const [users, setUsers] = useState([]);
  const [isAddMemberOpen, setAddMemberOpen] = useState(false);
  const [roles, setRoles] = useState({
    RoleName: "",
    RoleId: "",
    UserId: "",
    FolderId: folderid,
  });
  const [selectedUsers, setSelectedUsers] = useState([]);

  const handleAutocompleteChange = (event, newValue) => {
    setSelectedUsers(newValue);
  };

  const token = sessionStorage.getItem("token");
  if (token == null || token === "") {
    window.location.href = "./login";
  }
  const js = JSON.parse(token);
  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };

  /** Get all members that belong to the selected folder */
  const get_folder_members = async () => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        CreatedByUserId: data.datacreatedbyuserid,
        Id: folderid,
      };

      const response = await fetch(getTeamFolderMembers, {
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
          setMembers(data); // Ensure data is an array
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
  const get_all_users = async () => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        CreatedByUserId: data.datacreatedbyuserid,
      };

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
          setUsers(data); // Ensure data is an array
        } else {
          alert("API response is not an array:", data);
          return;
        }
      } else {
        alert("Failed to fetch Users:", response.statusText);
        return;
      }
    } catch (err) {
      alert(`error retrieving data ${err}`);
      return;
    }
  };
  const get_all_roles = async () => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        CreatedByUserId: data.datacreatedbyuserid,
        Id: folderid,
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
          setGetRoles(data); // Ensure data is an array
        } else {
          alert("API response is not an array:", data);
          return;
        }
      } else {
        alert("Failed to fetch Roles:", response.statusText);
        return;
      }
    } catch (err) {
      alert(`error retrieving data ${err}`);
      return;
    }
  };

  const showForm = () => {
    setAddMemberOpen(!isAddMemberOpen);
  };

  useEffect(() => {
    get_folder_members();
    get_all_users();
    get_all_roles();
  }, []);

  /** *********************************************
   * ***************Add a new member to a folder */
  const addMember = async (e) => {
    try {
      e.preventDefault();
      if (selectedUsers.length === 0) {
        alert("Please select at least one user.");
        return;
      }
      const newRoles = selectedUsers.map((user) => ({
        RoleID: roles.RoleId,
        EmpId: user.empId,
        FolderId: folderid,
        CreatedByUserId: data.datacreatedbyuserid,
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
      }));

      const headers = new Headers({ "Content-Type": "application/json" });
      const response = await fetch(addMemberToFolder, {
        method: "POST",
        headers: headers,
        body: JSON.stringify(newRoles),
      });
      if (response.statusText === "OK") {
        get_folder_members();
        return;
      }
    } catch (error) {
      alert(`Failed adding user to folder`);
    }
  };
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        justifyContent: "space-between",
        padding: "2px",
        width: "100%",
      }}
    >
      <Box>
        <Box
          sx={{
            display: "flex",
            //textTransform: "uppercase",
            marginBottom: "50px",
            justifyContent: "flex-end",
          }}
        >
          <Button
            variant="contained"
            sx={{
              backgroundColor: "green",
              display: isAddMemberOpen ? "none" : "block",
            }}
            onClick={showForm}
          >
            add members
          </Button>
          <Box
            sx={{
              display: isAddMemberOpen ? "block" : "none",
              width: "100%",
            }}
          >
            <Autocomplete
              multiple
              id="tags-outlined"
              options={users}
              getOptionLabel={(option) => option.displayName}
              value={selectedUsers}
              onChange={handleAutocompleteChange}
              filterSelectedOptions
              renderInput={(params) => (
                <TextField
                  {...params}
                  label=""
                  placeholder="Add members by display name"
                />
              )}
            />
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                width: "100%",
                mt: 2,
              }}
            >
              <Box sx={{ display: "flex", alignItems: "center" }}>
                <Typography sx={{ marginRight: "10px" }}>Role</Typography>
                <Box sx={{ display: "flex", mr: "20px", width: "200px" }}>
                  <Select
                    sx={{ height: "40px" }}
                    fullWidth
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    value={getroles.roleID}
                    label="Alias"
                    onChange={(e) => {
                      const selectedRole = getroles.find(
                        (role) => role.roleID === e.target.value
                      );
                      setRoles({
                        ...roles,
                        RoleId: e.target.value,
                        RoleName: selectedRole ? selectedRole.roleName : "",
                      });
                    }}
                  >
                    {getroles.length > 0
                      ? getroles.map((item, index) => (
                          <MenuItem key={index} value={item.roleID}>
                            {item.roleName}
                          </MenuItem>
                        ))
                      : null}
                  </Select>
                </Box>
              </Box>
              <Box sx={{ display: "flex", gap: 2 }}>
                <Button variant="outlined" onClick={showForm}>
                  Cancel
                </Button>
                <Button
                  variant="contained"
                  onClick={addMember}
                  sx={{ backgroundColor: "green" }}
                >
                  Add
                </Button>
              </Box>
            </Box>
          </Box>
        </Box>

        <Box
          sx={{
            flex: 1,
            textAlign: "right",
            display: "flex",
            flexDirection: "row",
            gap: 10,
            paddingLeft: "30px",
            justifyContent: "space-between",
          }}
        >
          <Typography variant="body1" fontWeight="bold">
            Members ({members.length})
          </Typography>
        </Box>

        <Box
          sx={{
            display: "flex",
            flexWrap: "wrap",
            justifyContent: "space-between",
            gap: 2,
            padding: 2,
            backgroundColor: "#f0f0f0",
            borderRadius: 2,
            m: 4,
          }}
        >
          {members.map((name, index) => (
            <Box
              key={index}
              sx={{
                flex: "1 0 21%", // Adjust this value to control the number of names per row
                textAlign: "center",
                padding: 1,
                margin: 1,
                backgroundColor: "#ffffff",
                borderRadius: 1,
                boxShadow: "0 2px 5px rgba(0,0,0,0.1)",
                "&:hover": {
                  backgroundColor: "#f5f5f5",
                },
              }}
            >
              <Typography variant="body1" sx={{ fontWeight: "bold" }}>
                {name.displayName}
              </Typography>
            </Box>
          ))}
        </Box>
      </Box>
    </div>
  );
};

export default Members;
