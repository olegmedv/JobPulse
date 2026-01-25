import { Authenticated, Refine } from "@refinedev/core";
// import { DevtoolsPanel, DevtoolsProvider } from "@refinedev/devtools";
import { RefineKbar, RefineKbarProvider } from "@refinedev/kbar";
import {
  AuthPage,
  ThemedLayout,
  useNotificationProvider,
} from "@refinedev/antd";
import "@refinedev/antd/dist/reset.css";

import routerProvider, {
  DocumentTitleHandler,
  UnsavedChangesNotifier,
} from "@refinedev/react-router";
import { liveProvider } from "@refinedev/supabase";
import { App as AntdApp } from "antd";
import { BrowserRouter, Navigate, Outlet, Route, Routes } from "react-router";
import { ColorModeContextProvider } from "./contexts/color-mode";
import authProvider from "./providers/auth";
import { dataProvider } from "./providers/data";
import { supabaseClient } from "./providers/supabase-client";
import { JobsList, JobsCreate, JobsEdit, JobsShow } from "./pages/jobs";

function App() {
  return (
    <BrowserRouter>
      <RefineKbarProvider>
        <ColorModeContextProvider>
          <AntdApp>
            {/* <DevtoolsProvider> */}
            <Refine
              dataProvider={dataProvider}
              liveProvider={liveProvider(supabaseClient)}
              authProvider={authProvider}
              routerProvider={routerProvider}
              notificationProvider={useNotificationProvider}
              options={{
                syncWithLocation: true,
                warnWhenUnsavedChanges: true,
                projectId: "gKHvxj-63dDym-5B4Y7G",
              }}
              resources={[
                {
                  name: "jobs",
                  list: "/jobs",
                  create: "/jobs/create",
                  edit: "/jobs/edit/:id",
                  show: "/jobs/show/:id",
                  meta: {
                    label: "Jobs",
                  },
                },
              ]}
            >
              <Routes>
                {/* Protected routes */}
                <Route
                  element={
                    <Authenticated
                      key="authenticated-routes"
                      fallback={<Navigate to="/login" />}
                    >
                      <ThemedLayout>
                        <Outlet />
                      </ThemedLayout>
                    </Authenticated>
                  }
                >
                  <Route index element={<div>Dashboard - Coming Soon</div>} />
                  <Route path="/jobs" element={<JobsList />} />
                  <Route path="/jobs/create" element={<JobsCreate />} />
                  <Route path="/jobs/edit/:id" element={<JobsEdit />} />
                  <Route path="/jobs/show/:id" element={<JobsShow />} />
                </Route>

                {/* Auth pages */}
                <Route
                  element={
                    <Authenticated key="auth-pages" fallback={<Outlet />}>
                      <Navigate to="/" />
                    </Authenticated>
                  }
                >
                  <Route path="/login" element={<AuthPage type="login" />} />
                  <Route path="/register" element={<AuthPage type="register" />} />
                  <Route path="/forgot-password" element={<AuthPage type="forgotPassword" />} />
                  <Route path="/update-password" element={<AuthPage type="updatePassword" />} />
                </Route>
              </Routes>
              <RefineKbar />
              <UnsavedChangesNotifier />
              <DocumentTitleHandler />
            </Refine>
            {/* <DevtoolsPanel />
            </DevtoolsProvider> */}
          </AntdApp>
        </ColorModeContextProvider>
      </RefineKbarProvider>
    </BrowserRouter>
  );
}

export default App;