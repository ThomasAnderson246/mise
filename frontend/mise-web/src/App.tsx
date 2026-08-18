import { Routes, Route, useParams /*useNavigate*/ } from "react-router-dom";
import ProtectedRoute from "./components/ProtectedRoute";
import LoginPage from "./pages/LoginPage";
//import { useAuth } from './context/AuthContext'
import { Layout } from "./components/Layout";
import DashBoardPage from "./pages/DashboardPage";
import RecipesPage from "./pages/RecipesPage";
import RecipeDetailPage from "./pages/RecipeDetailPage";
import RecipeEditorPage from "./pages/RecipeEditorPage";
import CookingModePage from "./pages/CookingModePage";
import PrepListPage from "./pages/PrepListPage";
import PrepListDetailPage from "./pages/PrepListDetailPage";

function SlugRoutes() {
  const { slug } = useParams<{ slug: string }>();

  return (
    <ProtectedRoute slug={slug!}>
      <Routes>
        <Route element={<Layout />}>
          <Route path="dashboard" element={<DashBoardPage />} />
          <Route path="recipes/new" element={<RecipeEditorPage />} />
          <Route path="recipes/:recipeId/edit" element={<RecipeEditorPage />} />
          <Route path="recipes/:recipeId" element={<RecipeDetailPage />} />
          <Route path="recipes" element={<RecipesPage />} />
          <Route path="prep-lists" element={<PrepListPage />} />
          <Route
            path="prep-lists/:prepListId"
            element={<PrepListDetailPage />}
          />
        </Route>
        <Route path="recipes/:recipeId/cook" element={<CookingModePage />} />
      </Routes>
    </ProtectedRoute>
  );
}

function App() {
  return (
    <Routes>
      <Route path="/:slug/login" element={<LoginPage />} />
      <Route path="/:slug/*" element={<SlugRoutes />} />
      <Route path="*" element={<div>404</div>} />
    </Routes>
  );
}

export default App;
