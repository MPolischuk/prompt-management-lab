import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';

export function AppLayout() {
  return (
    <div className="flex min-h-screen bg-gray-950 text-white">
      <Sidebar />
      <Outlet />
    </div>
  );
}
