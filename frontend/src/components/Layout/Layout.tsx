import React from 'react';
import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import './Layout.css';

export const Layout: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const isActiveLink = (path: string) => {
    return location.pathname === path;
  };

  return (
    <div className="layout">
      <nav className="navbar">
        <div className="navbar-brand">
          <h1>InovaSaúde</h1>
        </div>
        <div className="navbar-menu">
          <Link
            to="/dashboard"
            className={isActiveLink('/dashboard') ? 'active' : ''}
          >
            Dashboard
          </Link>
          <Link
            to="/ubs"
            className={isActiveLink('/ubs') ? 'active' : ''}
          >
            UBS
          </Link>
          <Link
            to="/despesas"
            className={isActiveLink('/despesas') ? 'active' : ''}
          >
            Despesas
          </Link>
          <Link
            to="/relatorios"
            className={isActiveLink('/relatorios') ? 'active' : ''}
          >
            Relatórios
          </Link>
        </div>
        <div className="navbar-user">
          <div className="navbar-user-info">
            <span>{user?.nome}</span>
            <span className="user-role">{user?.roles.join(', ')}</span>
          </div>
          <button onClick={handleLogout} className="btn btn-outline">
            Sair
          </button>
        </div>
      </nav>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
};
