<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Techspire_LMS.Pages.Default" %>


<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <%-- INTERNAL CSS (F3) — page-specific rules that only this page needs.
         The general design system (colours, buttons, cards, forms) lives in
         the EXTERNAL stylesheet (Content/site.css, linked once from
         Site.master so every page shares it); a handful of one-off tweaks
         for this specific page's hero banner is exactly the right amount of
         CSS to keep internal rather than adding one-page-only classes to
         the shared file. Elsewhere in the templates (see any admin page's
         style="..." attributes) INLINE style is used for small, truly
         one-off tweaks a class isn't worth naming — so between this block,
         site.css, and those inline attributes, the app demonstrates all
         three CSS placement methods plus their combination on one page. --%>
    <style type="text/css">
        .hero {
            background: linear-gradient(135deg, var(--brand), var(--brand-dark));
            color: #fff;
            border-radius: var(--radius);
            padding: 2.5rem 2rem;
            margin-bottom: 2rem;
        }
        .hero .page-title, .hero .page-subtitle { color: #fff; }
        .hero .page-subtitle { opacity: 0.9; }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hero">
        <h1 class="page-title">Welcome</h1>
        <p class="page-subtitle">Browse our featured courses below, or see the full catalogue.</p>
    </section>

    <section aria-label="Featured courses">
        <asp:Repeater ID="rptFeatured" runat="server">
            <HeaderTemplate><div class="card-grid"></HeaderTemplate>
            <ItemTemplate>
                <article class="card">
                    <img class="card-thumb" src="<%# ResolveUrl((string)Eval("ThumbnailOrDefault")) %>" alt="" />
                    <div class="card-body">
                        <span class="badge"><%# Eval("CategoryName") %></span>
                        <h3 class="card-title"><%# Eval("Title") %></h3>
                        <p class="card-meta"><%# Eval("DifficultyLevel") %> · <%# Eval("DurationDisplay") %></p>
                        <p class="card-desc"><%# Eval("ShortDescription") %></p>
                        <a class="btn btn-outline btn-small"
                           href="<%# ResolveUrl("~/Pages/CourseDetails.aspx?id=") + Eval("CourseID") %>">View course</a>
                    </div>
                </article>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>

        <asp:Literal ID="litEmpty" runat="server" Visible="false">
            <p>Check back soon, we will keep posting.</p>
        </asp:Literal>
    </section>
</asp:Content>
