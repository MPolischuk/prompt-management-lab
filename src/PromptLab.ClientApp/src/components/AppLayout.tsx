import { useEffect, useState } from 'react';
import { Menu } from 'lucide-react';
import { Outlet, useLocation } from 'react-router-dom';
import { Sidebar } from './Sidebar';

export function AppLayout() {
  const [mobileNavOpen, setMobileNavOpen] = useState(false);
  const location = useLocation();

  useEffect(() => {
    setMobileNavOpen(false);
  }, [location.pathname]);

  return (
    <div className="flex min-h-screen bg-gray-950 text-white">
      {mobileNavOpen && (
        <button
          type="button"
          aria-label="Cerrar menú"
          className="fixed inset-x-0 bottom-0 top-14 z-40 bg-black/60 md:hidden"
          onClick={() => setMobileNavOpen(false)}
        />
      )}

      <header className="fixed top-0 left-0 right-0 z-50 flex h-14 items-center gap-3 border-b border-gray-800 bg-gray-950 px-4 md:hidden">
        <button
          type="button"
          aria-expanded={mobileNavOpen}
          aria-controls="app-sidebar"
          className="inline-flex h-10 w-10 items-center justify-center rounded-lg text-gray-200 hover:bg-gray-800 focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500"
          onClick={() => setMobileNavOpen((o) => !o)}
        >
          <Menu className="h-6 w-6" aria-hidden />
        </button>
        <span className="text-sm font-semibold tracking-wide text-white">PromptLab</span>
      </header>

      <Sidebar mobileOpen={mobileNavOpen} onMobileClose={() => setMobileNavOpen(false)} />

      <div className="flex min-h-0 flex-1 flex-col min-w-0 pt-14 md:pt-0">
        <Outlet />
      </div>
    </div>
  );
}
