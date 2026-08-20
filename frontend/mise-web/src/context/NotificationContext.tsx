import { createContext, useContext, useState, useCallback } from "react";
import type { ReactNode } from "react";
import { getUnreadNotifications } from "@/api/notificationApi";

interface NotificationContextType {
  unreadCount: number;
  fetchUnreadCount: (token: string) => Promise<void>;
  decrementUnread: () => void;
  clearUnread: () => void;
}

const NotificationContext = createContext<NotificationContextType | null>(null);

export function NotificationProvider({ children }: { children: ReactNode }) {
  const [unreadCount, setUnreadCount] = useState(0);

  const fetchUnreadCount = useCallback(async (token: string) => {
    try {
      const unread = await getUnreadNotifications(token);
      setUnreadCount(unread.length);
    } catch {
      // fail silently
    }
  }, []);

  function decrementUnread() {
    setUnreadCount((prev) => Math.max(0, prev - 1));
  }

  function clearUnread() {
    setUnreadCount(0);
  }

  return (
    <NotificationContext.Provider
      value={{
        unreadCount,
        fetchUnreadCount,
        decrementUnread,
        clearUnread,
      }}
    >
      {children}
    </NotificationContext.Provider>
  );
}

export function useNotifications() {
  const context = useContext(NotificationContext);
  if (!context)
    throw new Error(
      "useNotifications must be used within NotificationProvider.",
    );

  return context;
}
