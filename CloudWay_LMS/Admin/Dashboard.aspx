<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="Dashboard.aspx.cs" Inherits="CloudWay_LMS.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 2rem;">
        <h1 class="page-title" style="margin:0;">Dashboard Overview</h1>
        <div style="display:flex; gap:1rem;">
            <a href="<%= ResolveUrl("~/Admin/ManageUsers.aspx") %>" class="btn btn-outline btn-small">Manage Users</a>
            <a href="<%= ResolveUrl("~/Admin/ManageCourses.aspx") %>" class="btn btn-accent btn-small">Manage Courses</a>
        </div>
    </div>
    
    <style>
        .metric-card {
            padding: 1.75rem !important; 
            display: flex !important; 
            justify-content: space-between; 
            align-items: flex-start !important;
            flex-direction: row !important;
            gap: 0 !important;
        }
        .metric-icon {
            transition: all 0.3s ease;
        }
        .metric-card:hover .metric-icon-accent {
            background: var(--accent) !important;
            color: #ffffff !important;
            transform: scale(1.15) rotate(10deg);
        }
        .metric-card:hover .metric-icon-brand {
            background: var(--brand) !important;
            color: #ffffff !important;
            transform: scale(1.15) rotate(10deg);
        }
        .metric-card:hover .metric-icon-warning {
            background: #F59E0B !important;
            color: #ffffff !important;
            transform: scale(1.15) rotate(10deg);
        }
        .metric-card:hover .metric-icon-danger {
            background: var(--danger) !important;
            color: #ffffff !important;
            transform: scale(1.15) rotate(10deg);
        }
        .hero-icon {
            display: block;
        }
        @media (max-width: 768px) {
            .hero-icon { display: none !important; }
        }
    </style>

    <div class="card-grid" style="grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 1.5rem; margin-bottom: 2.5rem;">
        <!-- Metric Card 1 -->
        <div class="card card-clickable metric-card" onclick="window.location.href='<%= ResolveUrl("~/Admin/ManageUsers.aspx") %>';" style="border-left: 4px solid var(--accent);">
            <div>
                <p style="margin: 0; color: var(--text-muted); font-size: 13px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em;">Total Users</p>
                <h2 style="margin: 0.25rem 0 0; font-size: 2.25rem; color: var(--text); font-family: var(--font-heading);"><%= TotalUsers %></h2>
            </div>
            <div class="metric-icon metric-icon-accent" style="background: rgba(43, 209, 197, 0.15); width: 48px; height: 48px; border-radius: 12px; display:flex; align-items:center; justify-content:center; color: var(--accent);">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>
            </div>
        </div>

        <!-- Metric Card 2 -->
        <div class="card card-clickable metric-card" onclick="window.location.href='<%= ResolveUrl("~/Admin/ManageCourses.aspx") %>';" style="border-left: 4px solid var(--brand);">
            <div>
                <p style="margin: 0; color: var(--text-muted); font-size: 13px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em;">Total Courses</p>
                <h2 style="margin: 0.25rem 0 0; font-size: 2.25rem; color: var(--text); font-family: var(--font-heading);"><%= TotalCourses %></h2>
            </div>
            <div class="metric-icon metric-icon-brand" style="background: rgba(11, 37, 69, 0.1); width: 48px; height: 48px; border-radius: 12px; display:flex; align-items:center; justify-content:center; color: var(--brand);">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path></svg>
            </div>
        </div>

        <!-- Metric Card 3 -->
        <div class="card card-clickable metric-card" onclick="window.location.href='<%= ResolveUrl("~/Admin/ManageCategories.aspx") %>';" style="border-left: 4px solid #F59E0B;">
            <div>
                <p style="margin: 0; color: var(--text-muted); font-size: 13px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em;">Categories</p>
                <h2 style="margin: 0.25rem 0 0; font-size: 2.25rem; color: var(--text); font-family: var(--font-heading);"><%= TotalCategories %></h2>
            </div>
            <div class="metric-icon metric-icon-warning" style="background: rgba(245, 158, 11, 0.15); width: 48px; height: 48px; border-radius: 12px; display:flex; align-items:center; justify-content:center; color: #F59E0B;">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="8" y1="6" x2="21" y2="6"></line><line x1="8" y1="12" x2="21" y2="12"></line><line x1="8" y1="18" x2="21" y2="18"></line><line x1="3" y1="6" x2="3.01" y2="6"></line><line x1="3" y1="12" x2="3.01" y2="12"></line><line x1="3" y1="18" x2="3.01" y2="18"></line></svg>
            </div>
        </div>

        <!-- Metric Card 4 -->
        <div class="card card-clickable metric-card" onclick="window.location.href='<%= ResolveUrl("~/Admin/ViewFeedback.aspx") %>';" style="border-left: 4px solid var(--danger);">
            <div>
                <p style="margin: 0; color: var(--text-muted); font-size: 13px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em;">Feedback</p>
                <h2 style="margin: 0.25rem 0 0; font-size: 2.25rem; color: var(--text); font-family: var(--font-heading);"><%= TotalFeedback %></h2>
            </div>
            <div class="metric-icon metric-icon-danger" style="background: rgba(220, 53, 69, 0.15); width: 48px; height: 48px; border-radius: 12px; display:flex; align-items:center; justify-content:center; color: var(--danger);">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"></path></svg>
            </div>
        </div>
    </div>

    <div class="card" style="background: linear-gradient(135deg, var(--brand), var(--brand-dark)); color: white; border: none; padding: 2.5rem; display: flex; align-items: center; justify-content: space-between; border-radius: var(--radius); margin-bottom: 2rem;">
        <div>
            <h3 style="margin-top: 0; margin-bottom: 0.75rem; font-size: 1.75rem; font-family: var(--font-heading); font-weight: 600; letter-spacing: -0.01em; color: white !important;">Welcome to the CloudWay LMS Command Center</h3>
            <p style="margin: 0; opacity: 0.9; font-size: 1.1rem; max-width: 600px; font-family: var(--font-sans); line-height: 1.6;">
                Use the navigation sidebar to manage the platform's course catalog, user enrollments, categories, and incoming student feedback.
            </p>
        </div>
        <div class="hero-icon" style="opacity: 0.15;">
            <svg width="120" height="120" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="3" width="20" height="14" rx="2" ry="2"></rect><line x1="8" y1="21" x2="16" y2="21"></line><line x1="12" y1="17" x2="12" y2="21"></line></svg>
        </div>
    </div>
</asp:Content>
