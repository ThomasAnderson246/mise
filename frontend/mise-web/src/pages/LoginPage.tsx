import { useEffect, useState } from "react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import { getTenantBySlug } from "../api/tenantApi";
import { getPermissions, Login } from "../api/authApi";
import { useAuth } from "../context/AuthContext";
import type { TenantResponse } from "../api/tenantApi";

function LoginPage() {
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const { setUser, isAuthenticated } = useAuth();

  const [tenant, setTenant] = useState<TenantResponse | null>(null);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [redirectMessage, setRedirectMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [tenantLoading, setTenantLoading] = useState(true);

  // if authenticated, go straight to the dashboard
  useEffect(() => {
    if (isAuthenticated) {
      navigate(`/${slug}/dashboard`, { replace: true });
    }
  }, [location.state]);

  useEffect(() => {
    if (location.state?.message) {
      setRedirectMessage(location.state.message);
    }
  }, [location.state]);

  //fetch tenant on mount ... this will get the tenant name and branding

  useEffect(() => {
    if (!slug) return;
    setTenantLoading(true);
    getTenantBySlug(slug)
      .then(setTenant)
      .catch(() => setError("Restaurant not found. PLease check your link."))
      .finally(() => setTenantLoading(false));
  }, [slug]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!tenant) return;
    setError(null);
    setLoading(true);

    try {
      const response = await Login({
        email,
        password,
        tenantId: tenant.tenantId,
      });

      const permissions = await getPermissions(response.token);
      console.log("Permissions loaded: ", permissions);
      setUser({
        token: response.token,
        userId: response.userId,
        email: response.email,
        firstName: response.firstName,
        lastName: response.lastName,
        tenantId: response.tenantId,
        role: response.role,
        permissions: permissions,
      });
      navigate(`/${slug}/dashboard`, { replace: true });
    } catch {
      setError("Invalid email or password.");
    } finally {
      setLoading(false);
    }
  }

  if (tenantLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-white">
        <p className="text-[#6B1A2B] font-medium">Loading....</p>
      </div>
    );
  }

  if (!tenant) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-white">
        <p className="text-red-600 font-medium">Restaurant not found.</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#6B1A2B]">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-md px-10 py-12">
        <div className="mb-8 text-center">
          <h1 className="text-4xl font-bold text-[#6B1A2B] tracking-tight">
            Mise
          </h1>
          <p className="text-sm text-gray-500 mt-1">{tenant.name}</p>
        </div>

        {redirectMessage && (
          <div className="mb-4 px-4 py-3 rounded-lg bg-yellow-50 border border-[#C9972C] text-[#C9972C] text-sm text-center">
            {redirectMessage}
          </div>
        )}

        {error && (
          <div className="mb-4 px-4 py-3 rounded-lg bg-red-50 border border-[#6B1A2B] text-[#6B1A2B] text-sm text-center">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-[#6B1A2B] focus:border-transparent"
              placeholder="you@restaurant.com"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Password
            </label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm focus:outline-none focus: ring-2 focus:ring-[#6B1A2B] focus: border-transparent"
              placeholder="********"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-2.5 px-4 bg-[#6B1A2B] hover:bg-[#5a1624] text-white fond-semibold rounded-lg text-sm transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? "Signing in..." : "Sign In"}
          </button>
        </form>

        <p className="mt-8 text-center text-xs text-gray-400">
          Mise - Built for profession kitchens
        </p>
      </div>
    </div>
  );
}

export default LoginPage;
