import React from 'react';
import { useAuth } from '../../contexts/AuthContext';
import './Dashboard.css';

export const Dashboard: React.FC = () => {
  const { user } = useAuth();

  return (
    <div className="dashboard">
      <h1>Bem-vindo, {user?.nome}!</h1>
      <p>Painel de controle do InovaSaúde</p>

      <div className="dashboard-grid">
        <div className="dashboard-card">
          <h3>Total de UBS</h3>
          <div className="card-value">-</div>
          <p>Unidades cadastradas</p>
        </div>

        <div className="dashboard-card">
          <h3>Total de Despesas</h3>
          <div className="card-value">R$ -</div>
          <p>No período atual</p>
        </div>

        <div className="dashboard-card">
          <h3>Despesas Pendentes</h3>
          <div className="card-value">-</div>
          <p>Aguardando aprovação</p>
        </div>

        <div className="dashboard-card">
          <h3>Relatórios</h3>
          <div className="card-value">-</div>
          <p>Disponíveis para análise</p>
        </div>
      </div>

      <div className="dashboard-info">
        <h2>Sistema de Gestão de Despesas - UBS</h2>
        <p>
          Este é o sistema InovaSaúde para gerenciamento de despesas das Unidades 
          Básicas de Saúde. Use o menu acima para navegar entre as funcionalidades.
        </p>
        <ul>
          <li><strong>Dashboard:</strong> Visão geral do sistema</li>
          <li><strong>UBS:</strong> Gerenciamento de unidades de saúde</li>
          <li><strong>Despesas:</strong> Registro e acompanhamento de gastos</li>
        </ul>
      </div>
    </div>
  );
};
