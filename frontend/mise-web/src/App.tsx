import {Routes, Route, useParams, useNavigate } from 'react-router-dom'
import ProtectedRoute from './components/ProtectedRoute'
import LoginPage from './pages/LoginPage'
import { useAuth } from './context/AuthContext'

function SlugDashboard() {
  const {slug} = useParams<{slug: string}>()
  const {logout, user} = useAuth()
  const navigate = useNavigate()

  function handleLogout() {
    logout()
    navigate(`/${slug}/login`, {replace: true})
  }
  return(
    <ProtectedRoute slug={slug!}>
      <div className='min-h-screen bg-white p-8'>
        <div className="flex items-center justify-between mb-8">
          <h1 className="text-2x1 font-bold text-[#6b1a2b]">Mise</h1>
          <div className="flex items-center gap-4">
            <span className="text-sm text-gray-500">
              {user?.firstName} {user?.lastName}
            </span>
            <button onClick={handleLogout} className='px-4 py-2 text-sm font-medium text-white bg-[#6b1a2b] round-lg hover:bg-[#5a1624] transition-colors'>
              Log Out
            </button>
          </div>
        </div>
        <p className="text-gray-500">Dashboard coming soon....</p>
      </div>
    </ProtectedRoute>
  )
}

function App() {
  return (
    <Routes>
      <Route path="/:slug/login" element={<LoginPage/>}/>
      <Route path="/:slug/dashboard" element={<SlugDashboard/>}/>
      <Route path="*" element={<div>404</div>}/>

    </Routes>
  )
}

export default App