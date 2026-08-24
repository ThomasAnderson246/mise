import { useState, useEffect } from "react";
import { useAuth } from "@/context/AuthContext";
import { inviteUser } from "@/api/userApi";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { Button } from "../ui/button";
import { toast } from "sonner";
import { inputClass, selectClass } from "@/lib/styles";
import { authHeaders, BASE_URL } from "@/api/config";
import axios from "axios";
import type { InviteUserRequest } from "@/api/userApi";

interface RoleOption {
  roleId: string;
  name: string;
}

interface InviteUserModalProps {
  open: boolean;
  onClose: () => void;
  onInvited: () => void;
}

export function InviteUserModal({
  open,
  onClose,
  onInvited,
}: InviteUserModalProps) {
  const { user } = useAuth();

  const [email, setEmail] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [unitPreference, setUnitPreference] = useState("metric");
  const [selectedRoleId, setSelectedRoleId] = useState("");
  const [roles, setRoles] = useState<RoleOption[]>([]);
  const [inviting, setInviting] = useState(false);

  //temp password display... will eventually be migrated to email. exists for testing only
  const [tempPassword, setTempPassword] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    if (!open || !user?.token) return;

    axios
      .get(`${BASE_URL}/api/role`, authHeaders(user.token))
      .then((res) => setRoles(res.data.data ?? []))
      .catch(() => {});
  }, [open, user]);

  async function handleInvite() {
    if (!user?.token || !email.trim() || !firstName.trim() || !lastName.trim())
      return;
    setInviting(true);

    try {
      const request: InviteUserRequest = {
        email,
        firstName,
        lastName,
        unitPreference,
        roleIds: selectedRoleId ? [selectedRoleId] : [],
      };
      const result = await inviteUser(user.token, request);
      setTempPassword(result.temporaryPassword);
      onInvited();
    } catch {
      toast.error("Failed to invite user.");
    } finally {
      setInviting(false);
    }
  }

  async function handleCopy() {
    if (!tempPassword) return;
    await navigator.clipboard.writeText(tempPassword);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  function handleClose() {
    setEmail("");
    setFirstName("");
    setLastName("");
    setUnitPreference("metric");
    setSelectedRoleId("");
    setTempPassword(null);
    setCopied(false);
    onClose();
  }

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>
            {tempPassword ? "UserInvited" : "Invite user"}
          </DialogTitle>
        </DialogHeader>

        {tempPassword ? (
          <div className="space-y-4 mt-4">
            <p className="text-sm text-foreground">
              User invited successfully. Share this temporary password with
              them. They'll be prompted to change it on first login.
            </p>
            <div className="flex items-center gap-2 p-3 bg-muted rounded-lg border border-border">
              <code className="flex-1 text-sm font-mono text-foreground">
                {tempPassword}
              </code>
              <Button
                variant="outline"
                onClick={handleCopy}
                className="text-xs h-8 px-3 flex-shrink-0"
              >
                {copied ? "Copied!" : "Copy"}
              </Button>
            </div>
            <Button
              onClick={handleClose}
              className="w-full bg-primary text-primary-foreground"
            >
              Done
            </Button>
          </div>
        ) : (
          <div className="space-y-3 mt-4">
            <div className="flex gap-2">
              <div className="flex-1">
                <label className="block text-sm font-medium text-foreground mb-1">
                  First name
                </label>
                <input
                  type="text"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  className={inputClass}
                  placeholder="First name"
                />
              </div>

              <div className="flex-1">
                <label className="block text-sm font-medium text-foreground mb-1">
                  Last name
                </label>
                <input
                  type="text"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  className={inputClass}
                  placeholder="Last name"
                />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium text-foreground mb-1">
                Email
              </label>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className={inputClass}
                placeholder="email@restaurant.com"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-foreground mb-1">
                Role
              </label>
              <select
                value={selectedRoleId}
                onChange={(e) => setSelectedRoleId(e.target.value)}
                className={selectClass}
              >
                <option value="">No role assigned</option>
                {roles.map((r) => (
                  <option key={r.roleId} value={r.roleId}>
                    {r.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-foreground mb-1">
                Unit preference
              </label>
              <select
                value={unitPreference}
                onChange={(e) => setUnitPreference(e.target.value)}
                className={selectClass}
              >
                <option value="metric">Metric</option>
                <option value="imperial">Imperial</option>
              </select>
            </div>
            <div className="flex gap-2 pt-2">
              <Button
                onClick={handleInvite}
                disabled={
                  inviting ||
                  !email.trim() ||
                  !firstName.trim() ||
                  !lastName.trim()
                }
                className="flex-1 bg-primary text-primary-foreground"
              >
                {inviting ? "Inviting..." : "Send invite"}
              </Button>
              <Button variant="outline" onClick={handleClose}>
                Cancel
              </Button>
            </div>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}
