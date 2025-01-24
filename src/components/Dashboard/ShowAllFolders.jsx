import React, { useState, useEffect } from "react";
import "./Dashboard.css";
import { useNavigate } from "react-router-dom";
import { showAllFolders } from "../../config";
import moment from "moment";
import { DataGrid } from "@mui/x-data-grid";
import { Box, Typography } from "@mui/material";
import { Folder } from "@mui/icons-material";
import { styled, alpha } from "@mui/material/styles";

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
  //** Get session variable */
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

  //** handle RowDoubleClick on folder to view all files in folder */
  // const handleRowDoubleClick = async (params) => {
  //   const folderId = params.row.id;

  //   window.location.href = `./viewfiles?id=${folderId}`;

  // };

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
          // onRowDoubleClick={handleRowDoubleClick}
        />
      </div>
    </div>
  );
};

const ShowAllFolders = () => {
  const [folders, setFolders] = useState([]);
  const navigate = useNavigate();
  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (!token) {
      navigate("/login");
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
  //** Load all folders on page load and after insert */
  const Get_all_folders = async () => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: data.datatoken,
        SecretKey: data.datasecretkey,
        CreatedByUserId: data.datacreatedbyuserid,
      };

      const response = await fetch(showAllFolders, {
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
  useEffect(() => {
    Get_all_folders();
  }, []);
  return (
    <div>
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
        <Folders rows={folders} />
      </div>
    </div>
  );
};

export default ShowAllFolders;
