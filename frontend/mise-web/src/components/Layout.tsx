import { Outlet /*useParams*/ } from "react-router-dom";
import { Sidebar } from "./Sidebar";
import { BottomNav } from "./BottomNav";
import { Toaster } from "./ui/sonner";

export function Layout() {
  return (
    <>
      <div className="flex min-h-screen bg-background">
        <Sidebar />
        <main className="flex-1 flex flex-col min-w-0">
          <div className="flex-1 p-4 md:p=6 pb-20 md:pb-6">
            <Outlet />
          </div>
        </main>
        <BottomNav />
      </div>
      <Toaster />
    </>
  );
}
