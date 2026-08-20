import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import "./index.css";
import App from "./App.tsx";
import { AuthProvider } from "./context/AuthContext.tsx";
import { TimerProvider } from "./context/TimerContext.tsx";
import { NotificationProvider } from "./context/NotificationContext.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <TimerProvider>
          <NotificationProvider>
            <App />
          </NotificationProvider>
        </TimerProvider>
      </AuthProvider>
    </BrowserRouter>
  </StrictMode>,
);
