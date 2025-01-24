
var csharpurl = 'http://localhost:5210'; // Development Server
//var csharpurl = 'http://172.16.0.23:8081'; //Production Server

export const csharpUrl = csharpurl;
export const mediaUrl = csharpurl;

// APIs for Users
export const loginUser = csharpUrl + '/api/LoginUser/Getuser';
export const getUserAccounts = csharpUrl + '/api/LoginUser/Getusers';
export const createUserAccount = csharpUrl + '/api/LoginUser/AddUser';

// APIs for Folders
export const createFolder = csharpUrl + '/api/Folders/CreateFolder';
export const getFolders = csharpUrl + '/api/Folders/GetFolders';
export const showAllFolders = csharpUrl + '/api/Folders/ShowAllFolders';

// APIs for Files
export const createFile = csharpUrl + '/api/Files/CreateFile';
export const getFiles = csharpUrl + '/api/Files/GetFiles';

// APIs for Team Folders
export const createTeamFolder = csharpUrl + '/api/Folders/CreateTeamFolder';
export const getTeamFolders = csharpUrl + '/api/Folders/GetTeamFolders';

//APIs for Team Files
export const createTeamFile = csharpUrl + '/api/Files/CreateTeamFile';
export const getTeamFiles = csharpUrl + '/api/Files/GetTeamFiles';

// APIs for Roles
export const createRole = csharpUrl + '/api/Roles/CreateRole';
export const getRoles = csharpUrl + '/api/Roles/GetRoles';

// APIs for User Roles
export const createUserRole = csharpUrl + '/api/UserRoles/CreateUserRole';

// APIs for User SETTINGS
export const getTeamFoldersDetails = csharpUrl + '/api/Folders/GetTeamFolderDetails';
export const getTeamFolderMembers = csharpUrl + '/api/Folders/GetTeamFolderMembers';
export const addMemberToFolder = csharpUrl + '/api/Folders/AddTeamMembersToFolder';