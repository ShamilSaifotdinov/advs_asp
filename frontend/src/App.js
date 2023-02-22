import React from "react";
import { Routes, Route } from "react-router-dom";

import './App.css';

import Header from "./routes/Header"

import Main from "./routes/MainPage/Main"

import Login from "./routes/Profile/Login"
import Profile from "./routes/Profile/Profile";
import NewAdv from "./routes/Profile/NewAdv";

import AdvPage from "./routes/AdvPage"

function App() {
    return (     
      <div className="app">
        <Header />
        <Routes>
          <Route path="/" element={<Main />} />
          <Route path="/login" element={<Login />} />          
          <Route path="/new" element={<NewAdv />} />
          <Route path="/objects/:id" element={<AdvPage />} />
          <Route path="/profile" element={<Profile />} />
        </Routes>
      </div>
    );
  }

// import logo from './logo.svg';
// import './App.css';

// function App() {
//   return (
//     <div className="App">
//       <header className="App-header">
//         <img src={logo} className="App-logo" alt="logo" />
//         <p>
//           Edit <code>src/App.js</code> and save to reload.
//         </p>
//         <a
//           className="App-link"
//           href="https://reactjs.org"
//           target="_blank"
//           rel="noopener noreferrer"
//         >
//           Learn React
//         </a>
//       </header>
//     </div>
//   );
// }

export default App;
