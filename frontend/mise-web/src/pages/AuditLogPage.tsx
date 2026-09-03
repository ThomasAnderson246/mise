import { useEffect, useState, useMemo } from "react";
import { useAuth } from "@/context/AuthContext";
import { getAuditLogs } from "@/api/auditLogApi";
import { getUsers } from "@/api/userApi";
import { PageHeader } from "@/components/PageHeader";
import { AuditLogFilters } from "@/components/audit/AuditLogFilters";
import { AuditLogEntry } from "@/components/audit/AuditLogEntry";
import { getActionGroup, getFromDate } from "@/config/auditLog";
import { toast } from "sonner";
import type { AuditLogEntry as AuditLogEntryType } from "@/api/auditLogApi";
import type { UserItem } from "@/api/userApi";

export default function AuditLogPage() {
  const { user } = useAuth();

  const [logs, setLogs] = useState<AuditLogEntryType[]>([]);
  const [users, setUsers] = useState<UserItem[]>([]);
  const [loading, setLoading] = useState(true);

  const [dateRange, setDateRange] = useState("7days");
  const [entityType, setEntityType] = useState("all");
  const [selectedGroups, setSelectedGroups] = useState<string[]>([]);
  const [selectedUser, setSelectedUser] = useState("all");

  useEffect(() => {
    if (!user?.token) return;

    async function load() {
      try {
        const [logData, userData] = await Promise.all([
          getAuditLogs(user!.token),
          getUsers(user!.token),
        ]);
        setLogs(logData);
        setUsers(userData);
      } catch {
        toast.error("Failed to load audit log.");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [user]);

  function toggleGroup(group: string) {
    setSelectedGroups((prev) =>
      prev.includes(group) ? prev.filter((g) => g !== group) : [...prev, group],
    );
  }

  const filtered = useMemo(() => {
    const fromDate = getFromDate(dateRange);

    return logs.filter((log) => {
      if (fromDate && new Date(log.performedAt) < fromDate) return false;
      if (entityType !== "all" && log.resource !== entityType) return false;
      if (
        selectedGroups.length > 0 &&
        !selectedGroups.includes(getActionGroup(log.action))
      )
        return false;
      if (selectedUser !== "all" && log.performedAt !== selectedUser)
        return false;
      return true;
    });
  }, [logs, dateRange, entityType, selectedGroups, selectedUser]);

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading audit log...</p>
      </div>
    );
  }

  return (
    <div>
      <PageHeader title="Audit Log" subtitle={`${filtered.length} entries`} />

      <AuditLogFilters
        dateRange={dateRange}
        entityType={entityType}
        selectedGroups={selectedGroups}
        selectedUser={selectedUser}
        users={users}
        onDateRangeChange={setDateRange}
        onEntityTypeChange={setEntityType}
        onGroupToggle={toggleGroup}
        onUserChange={setSelectedUser}
      />

      {filtered.length === 0 ? (
        <div className="text-center py-16">
          <p className="text-muted-foreground">
            No audit log entries match your filters.
          </p>
        </div>
      ) : (
        <div className="space-y-2">
          {filtered.map((log) => (
            <AuditLogEntry key={log.auditLogId} log={log} />
          ))}
        </div>
      )}
    </div>
  );
}
