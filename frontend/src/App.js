import React from "react";
import { Routes, Route } from "react-router-dom";

import './App.css';

import Header from "./Header"
import Main from "./routes/Main"
import Login from "./routes/Login"
import AdvPage from "./routes/AdvPage"
import NewAdv from "./routes/NewAdv";

function App() {
    return (     
      <div className="app">
        <Header />
        <Routes>
          <Route path="/" element={<Main />} />
          <Route path="/login" element={<Login />} />          
          <Route path="/new" element={<NewAdv />} />
          <Route path="/objects/:id" element={<AdvPage />} />
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
