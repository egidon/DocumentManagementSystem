import React, { useState } from "react";
import "./Login.css";
import TextField from "@mui/material/TextField";
import { Button, Card } from "@mui/material";
import DocuHubLogo from "../../images/DocuHubLogo.png";
import { loginUser } from "../../config.js";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [values, setValues] = useState({
    email: "",
    password: "",
  });
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    const response = await fetch(loginUser, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(values),
    });

    if (response.statusText === "OK") {
      const data = await response.json();
      if (data.length > 0) {
        if (Array.isArray(data)) {
          sessionStorage.setItem("token", JSON.stringify(data));
          navigate("/dashboard");
        }
      } else {
        alert("Incorrect Login Details, please check");
        return;
      }
    } else {
      alert("User not found");
      return;
    }
  };

  return (
    <div className="body">
      <div className="content">
        <div className="login-container">
          <img
            src={DocuHubLogo}
            alt="NIGCOMSAT"
            // style={{ width: "100%" }}
          />
          <Card>
            <div className="form-login-wrap">
              <div class="access-box">
                <form onSubmit={handleSubmit}>
                  <TextField
                    required
                    fullWidth
                    id="user_email"
                    label="Email"
                    defaultValue=""
                    sx={{ mt: 2 }}
                    onChange={(e) => {
                        setValues({ ...values, email: e.target.value });
                      }}
                  />
                  <TextField
                    id="user_password"
                    label="Password"
                    type="password"
                    autoComplete="current-password"
                    fullWidth
                    sx={{ mt: 2 }}
                    onChange={(e) => {
                        setValues({ ...values, password: e.target.value });
                      }}
                  />
                  <Button
                    variant="contained"
                    fullWidth
                    sx={{
                      mt: 2,
                      backgroundColor: "#10ac84",
                      textTransform: "capitalize",
                    }}
                    type="submit"
                  >
                    Login to your account
                  </Button>
                </form>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default Login;
