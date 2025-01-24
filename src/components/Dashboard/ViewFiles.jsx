import React, { useState, useEffect } from "react";
import "./Dashboard.css";
import { createFile, getFiles } from "../../config";
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
import { mediaUrl } from "../../config.js";
import IconButton from "@mui/material/IconButton";
import spinner from "../../images/spinner.gif";

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

const ViewFiles = () => {
  const [getFolderId, setGetFolderId] = useState("");
  const [files, setFiles] = useState([]);
  const [data, setData] = useState(null); // New state variable for data
  const [showImg, setShowImg] = useState(true);
  const [foldername, setFolderName] = useState("");

  const navigate = useNavigate();
  const location = useLocation();

  const token = sessionStorage.getItem("token");
  if (token == null || token === "") {
    navigate("/login");
  }

  useEffect(() => {
    const { folderId, foldername } = location.state;

    setGetFolderId(folderId);
    setFolderName(foldername);

    const js = JSON.parse(token);
    const dataObj = {
      isSuperUser: js[0].isSuperUser,
      datatoken: js[0].token,
      datasecretkey: js[0].secretKey,
      datacreatedbyuserid: js[0].empId,
    };
    setData(dataObj); // Set data in state
    fetchFiles(folderId, dataObj);
    setShowImg(false);
  }, []);

  const fetchFiles = async (folderId, dataObj) => {
    try {
      const send_values = {
        // Get all data to be sent to server and store in an object
        Token: dataObj.datatoken,
        SecretKey: dataObj.datasecretkey,
        FolderId: folderId,
        CreatedByUserId: dataObj.datacreatedbyuserid,
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
          setFiles(data); // Ensure data is an array
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
            width: "90%",
            padding: "10px",
          }}
        >
          <Files rows={files} foldername={foldername} />
        </div>
      )}
    </div>
  );
};

const Files = ({ rows, foldername }) => {
  const token = sessionStorage.getItem("token");
  // if (token == null || token === "") {
  //   window.location.href = "./login";
  // }
  const js = JSON.parse(token);

  const data = {
    isSuperUser: js[0].isSuperUser,
    datatoken: js[0].token,
    datasecretkey: js[0].secretKey,
    datacreatedbyuserid: js[0].empId,
  };
  return (
    <div className="viewCategoryDataTable">
      <div
        style={{
          width: "100%",
          display: "flex",
          fontSize: "16px",
          fontWeight: 600,
          marginBottom: "20px",
        }}
      >
        {foldername}
      </div>
      <div style={{ width: "100%" }}>
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
export default ViewFiles;
