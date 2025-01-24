import React, { useState, useEffect } from "react";
import { Box, Typography, Link } from "@mui/material";
import { getTeamFoldersDetails, getTeamFolderMembers } from "../../config.js";
import moment from "moment";

const TFDSettings = ({ folderid }) => {
  const [folders, setFolders] = useState([]);
  const [members, setMembers] = useState([]);
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

  /** Get all folder details of the selected folder */
  const get_folder_details = async () => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        CreatedByUserId: data.datacreatedbyuserid,
        Id: folderid,
      };

      const response = await fetch(getTeamFoldersDetails, {
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
  useEffect(() => {
    get_folder_details();
    get_folder_members();
  }, []);

  const names = [
    "John Doe",
    "Jane Smith",
    "Alice Johnson",
    "Robert Brown",
    "Emily Davis",
    "Michael Wilson",
    "Emma Thompson",
  ];

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "row",
        justifyContent: "space-between",
        padding: "2px",
        width: "100%",
      }}
    >
      {/* Box One */}
      {folders && folders.length > 0
        ? folders.map((val, index) => (
            <Box
              key={index}
              sx={{
                flex: 1,
                textAlign: "left",
                borderRight: "2px solid #D3D3D3",
                paddingRight: "80px",
              }}
            >
              <Box sx={{ marginBottom: "30px" }}>
                <Typography variant="body1" fontWeight="bold">
                  Name:
                </Typography>
                <Typography variant="body1">
                  {val.folderName} <Link href="#">[Edit]</Link>
                </Typography>
              </Box>

              <Box sx={{ marginBottom: "30px" }}>
                <Typography variant="body1" fontWeight="bold">
                  Description:
                </Typography>
                <Typography variant="body1">
                  {val.description}
                  <Link href="#"> [Edit]</Link>
                </Typography>
              </Box>

              <Box
                sx={{
                  marginBottom: "30px",
                  display: "flex",
                  flexDirection: "row",
                  justifyContent: "space-between",
                }}
              >
                <Typography
                  variant="body1"
                  fontWeight="bold"
                  sx={{ width: "150px", textAlign: "left" }}
                >
                  Type:
                </Typography>
                <Typography
                  variant="body1"
                  sx={{ flexGrow: 1, textAlign: "right" }}
                >
                  {val.permission === 1
                    ? "Private Team Folder"
                    : "Public Team Folder"}
                </Typography>
              </Box>

              <Box
                sx={{
                  marginBottom: "30px",
                  display: "flex",
                  flexDirection: "row",
                  justifyContent: "space-between",
                }}
              >
                <Typography
                  variant="body1"
                  fontWeight="bold"
                  sx={{ width: "150px", textAlign: "left" }}
                >
                  Created by:
                </Typography>
                <Typography
                  variant="body1"
                  sx={{ flexGrow: 1, textAlign: "right" }}
                >
                  {val.displayName} on{" "}
                  {moment(val.createdOnDate).format("MMMM Do, h:mm:ss a")}
                </Typography>
              </Box>

              <Box
                sx={{
                  display: "flex",
                  flexDirection: "row",
                  justifyContent: "space-between",
                  marginBottom: "30px",
                }}
              >
                <Typography
                  variant="body1"
                  fontWeight="bold"
                  sx={{ width: "150px", textAlign: "left" }}
                >
                  Location:
                </Typography>
                <Typography
                  variant="body1"
                  sx={{ flexGrow: 1, textAlign: "right" }}
                >
                  {val.location}
                </Typography>
              </Box>
            </Box>
          ))
        : null}

      {/* Box Two */}
      <Box>
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
            Members {(members.length)}
          </Typography>
          <Link href="#">+Add Members</Link>
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

export default TFDSettings;
