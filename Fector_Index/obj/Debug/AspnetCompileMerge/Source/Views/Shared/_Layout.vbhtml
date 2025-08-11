@Imports System.Diagnostics
@Code
    Dim stopWatch As StopWatch = Stopwatch.StartNew()
    
    Dim db As New Fector_Index.FectorEntities()
    Dim usr = Fector_Index.MsUser.GetAllUser(User.Identity.Name, db)
    Dim username As String = User.Identity.Name
    Dim FullName As String = usr.Fullname
    Dim Branch As String = usr.Branch.Name
    Dim RoleID As Integer = usr.UserlevelId
    Dim Role As String = usr.Userlevel.Description
    Dim CompleteRole As String = Branch & " (" & Role & ")"
    Dim RequestURL As String = Context.Request.FilePath
    Dim UserBranch As String = usr.BranchId
    
    Dim HOBranch As String = Fector_Index.MsSetting.GetSetting("Bank", "BranchId", "", "1")
    Dim yetReconsile As Integer = Fector_Index.MsMenu.GetYetReconsile(HOBranch, UserBranch, db)
    Dim undoneTransaction As Integer = Fector_Index.MsMenu.GetUndoneTransaction(HOBranch, UserBranch, db)
    Dim unapprovalTransaction As Integer = Fector_Index.MsMenu.GetUnapprovalTransaction(HOBranch, UserBranch, username, db)
    Dim unreviewTransaction As Integer = Fector_Index.MsMenu.GetUnreviewTransaction(HOBranch, UserBranch, db)
    Dim nonNPWP = Fector_Index.MsMenu.GetNonNPWP(HOBranch, UserBranch, db)
    
    'Dim HOBranch As String = 0
    'Dim yetReconsile As Integer = 0
    'Dim undoneTransaction As Integer = 0
    'Dim unapprovalTransaction As Integer = 0
    'Dim unreviewTransaction As Integer = 0
    'Dim nonNPWP = 0
    
    Dim CountYetAllTransaction As Integer = yetReconsile + undoneTransaction + unapprovalTransaction + unreviewTransaction
    Dim CountYetReconsile As Integer = yetReconsile
    Dim CountUndoneTransaction As Integer = undoneTransaction
    Dim CountUnapprovalTransaction As Integer = unapprovalTransaction
    Dim CountUnreviewTransaction As Integer = unreviewTransaction
    Dim CountNonNPWP As Integer = nonNPWP
    
    Dim CountBoth As Integer = 0
    Dim CountReconsile As Integer = 0
    Dim CountUndoneTrans As Integer = 0
    Dim CountUnapproveTransaction As Integer = 0
    Dim CountUnReview As Integer = 0
    
    Dim userAccessMenus = Fector_Index.MsUserAccess.GetMenus(RoleID)
    
    For Each menu As Fector_Index.MsMenu In userAccessMenus
        For Each submenu As Fector_Index.MsSubMenu In Fector_Index.MsUserAccess.GetSubMenus(RoleID, menu.MenuId)
            If submenu.Description = "View Transaction" Or submenu.Description = "Transaction Reconcile" Or submenu.Description = "Transaction Approval" Or submenu.Description = "Review Document Transaction" Then
                CountBoth += 1
                If submenu.Description = "View Transaction" Then
                    CountUndoneTrans += 1
                End If
                If submenu.Description = "Transaction Reconcile" Then
                    CountReconsile += 1
                End If
                If submenu.Description = "Transaction Approval" Then
                    CountUnapproveTransaction += 1
                End If
                If submenu.Description = "Review Document Transaction" Then
                    CountUnReview += 1
                End If
                'ElseIf submenu.Description = "Upload NPWP" Then
                '    CountNonNPWP += 1
            End If
        Next
    Next
    Dim initializationTime As Long = stopWatch.ElapsedMilliseconds
    Debug.WriteLine(initializationTime.ToString() + "ms : Layout Initialization Time")
    stopWatch.Restart()
End Code
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>@ViewData("Title") - Fector</title>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport">
    <!-- Font Awesome -->
    
    <!-- Ionicons -->
    
    @Styles.Render("~/Content/font-awesome.min.css")
    @Styles.Render("~/Content/ionicons.min.css")
    @Styles.Render("~/Content/css")
    @Styles.Render("~/Content/themes/base/css")
    @Styles.Render("~/AdminLTE/css")
    @Styles.Render("~/Morris/css")

    @Code
        Dim styleRenderTime As Long = stopWatch.ElapsedMilliseconds
        Debug.WriteLine(styleRenderTime.ToString() + "ms : Style Render Time")
        stopWatch.Restart()
    End Code
</head>
<!-- ADD THE CLASS fixed TO GET A FIXED HEADER AND SIDEBAR LAYOUT -->
<!-- the fixed layout is not compatible with sidebar-mini -->
<body class="hold-transition skin-blue fixed sidebar-mini sidebar-collapse">
    <!-- Site wrapper -->
    <div class="wrapper">
        <header class="main-header">
            <!-- Logo -->
            <a href="@Url.Action("Index", "Home")" class="logo">
                <!-- mini logo for sidebar mini 50x50 pixels -->
                <span class="logo-mini">FEC</span>
                <!-- logo for regular state and mobile devices -->
                <span class="logo-lg"><b>FECTOR</b></span>
            </a>
            <!-- Header Navbar: style can be found in header.less -->
            <nav class="navbar navbar-static-top" role="navigation">
                <!-- Sidebar toggle button-->
                <a href="#" class="sidebar-toggle" data-toggle="offcanvas" role="button">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </a>
                <div class="navbar-custom-menu">
                    <ul class="nav navbar-nav">
                        <!-- User Account: style can be found in dropdown.less -->
                        <li class="dropdown user user-menu">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                @FullName &nbsp;
                                <span class="hidden-xs"><span class="caret"></span></span>
                            </a>
                            <ul class="dropdown-menu">
                                <!-- User image -->
                                <li class="user-header">
                                    <p>
                                        @CompleteRole &nbsp;
                                    </p>
                                    <div class="pull-left">
                                        <a href="~/User/ChangePassword" class="btn btn-default btn-flat">Change Password</a>
                                    </div>
                                    <div class="pull-right">
                                        <a href="~/User/Logout" class="btn btn-default btn-flat">Logout</a>

                                    </div>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </div>
            </nav>
        </header>
        <!-- =============================================== -->
        <!-- Left side column. contains the sidebar -->
        <aside class="main-sidebar">
            <!-- sidebar: style can be found in sidebar.less -->
            <section class="sidebar">
                <!-- sidebar menu: : style can be found in sidebar.less -->
                <ul class="sidebar-menu">
                    <li class="header">MENU</li>
                    @For Each menu As Fector_Index.MsMenu In userAccessMenus
                        Dim List = Fector_Index.MsUserAccess.GetMenuName(RequestURL).ToList
                        If List.Count > 0 Then
                            If menu.Description = List.Item(0).MenuName Then
                                @<li class="treeview active">
                                    <a href="#">
                                        @If menu.Description = "Master" Then
                                            @<i class="fa fa-cogs"></i>
                                        End If
                                        @If menu.Description = "Deal" Then
                                            @<i class="fa fa-thumbs-o-up"></i>
                                        End If
                                        @If menu.Description = "User" Then
                                            @<i class="fa fa-user"></i>
                                        End If
                                        @If menu.Description = "Transaction" Then
                                            @<i class="fa fa-money"></i>
                                        End If
                                        @If menu.Description = "Download" Then
                                            @<i class="fa fa-download"></i>
                                        End If
                                        @If menu.Description = "Audit Trail" Then
                                            @<i class="fa fa-users"></i>
                                        End If
                                        @If menu.Description = "Inquiry" Then
                                            @<i class="fa fa-eye"></i>
                                        End If
                                        @If menu.Description = "Report" Then
                                            @<i class="fa fa-file-text-o"></i>
                                        End If
                                        @If menu.Description = "Master Approval" Then
                                            @<i class="fa fa-check"></i>
                                        End If
                                        <span>@menu.Description</span>
                                        @If menu.Description = "Transaction" Then
                                            @If yetReconsile > 0 Or undoneTransaction > 0 Or unreviewTransaction > 0 Then
                                                @<span class="pull-right-container">
                                                    @If CountBoth = 2 Then
                                                        @<span class="label label-danger">@CountYetAllTransaction</span>
                                                    ElseIf CountUndoneTrans = 1 Then
                                                        @<span class="label label-danger">@CountUndoneTransaction</span>
                                                    ElseIf CountReconsile = 1 Then
                                                        @<span class="label label-danger">@CountYetReconsile</span>
                                                    ElseIf CountUnapproveTransaction = 1 Then
                                                        @<span class="label label-danger">@CountUnapprovalTransaction</span>
                                                    ElseIf CountUnReview = 1 Then
                                                        @<span class="label label-danger">@CountUnreviewTransaction</span>
                                                    End If
                                                </span>
                                            End If
                                        ElseIf menu.Description = "Master" Then
                                            @If nonNPWP > 0 Then
                                                @<span class="pull-right-container">
                                                    <span class="label label-danger">@CountNonNPWP</span>
                                                </span>
                                            End If
                                        End If

                                        <i class="fa fa-angle-left pull-right"></i>
                                    </a>
                                    <ul class="treeview-menu ">
                                        @For Each submenu As Fector_Index.MsSubMenu In Fector_Index.MsUserAccess.GetSubMenus(RoleID, menu.MenuId)
                                    If RequestURL.Contains("/" & submenu.Link) Then
                                                @<li class="active" >
                                                    <a href="~/@submenu.Link">
                                                        <i class="fa fa-circle-o"></i> @submenu.Description
                                                        @If submenu.Description = "Transaction Reconcile" Then
                                                            @If yetReconsile > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@yetReconsile</span>
                                                            End If
                                                        ElseIf submenu.Description = "View Transaction" Then
                                                            @If undoneTransaction > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@undoneTransaction</span>
                                                            End If
                                                        ElseIf submenu.Description = "Transaction Approval" Then
                                                            @If unapprovalTransaction > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@unapprovalTransaction</span>
                                                            End If
                                                        ElseIf submenu.Description = "Review Transaction Document" Then
                                                            @If unreviewTransaction > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@unreviewTransaction</span>
                                                            End If
                                                        ElseIf submenu.Description = "Upload NPWP" Then
                                                            @If nonNPWP > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@nonNPWP</span>
                                                            End If
                                                        ElseIf submenu.Description = "Manual Reconcile" Then
                                                           @If yetReconsile > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@yetReconsile</span>
                                                           End If
                                                        End If
                                                    </a>
                                                </li>
                                            else
                                                @<li>
                                                    <a href="~/@submenu.Link">
                                                        <i class="fa fa-circle-o"></i> @submenu.Description
                                                        @If submenu.Description = "Transaction Reconcile" Then
                                                            @If yetReconsile > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@yetReconsile</span>
                                                            End If
                                                        ElseIf submenu.Description = "View Transaction" Then
                                                            @If undoneTransaction > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@undoneTransaction</span>
                                                            End If
                                                        ElseIf submenu.Description = "Transaction Approval" Then
                                                            @If unapprovalTransaction > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@unapprovalTransaction</span>
                                                            End If
                                                        ElseIf submenu.Description = "Review Transaction Document" Then
                                                            @If unreviewTransaction > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@unreviewTransaction</span>
                                                            End If
                                                        ElseIf submenu.Description = "Upload NPWP" Then
                                                            @If nonNPWP > 0 Then
                                                                @<span class=" pull-right-container label label-danger pull-right">@nonNPWP</span>
                                                            End If
                                                        End If
                                                    </a>
                                                </li>
                                            End If
                                        Next
                                    </ul>
                                </li>
                            Else
                                @<li class="treeview">
                                    <a href="#">
                                        @if menu.Description = "Master" then
                                            @<i class="fa fa-cogs"></i>
                                        End If
                                        @If menu.Description = "Deal" Then
                                            @<i class="fa fa-thumbs-o-up"></i>
                                        End If
                                        @If menu.Description = "User" Then
                                            @<i class="fa fa-user"></i>
                                        End If
                                        @If menu.Description = "Transaction" Then
                                            @<i class="fa fa-money"></i>
                                        End If
                                        @If menu.Description = "Download" Then
                                            @<i class="fa fa-download"></i>
                                        End If
                                        @If menu.Description = "Audit Trail" Then
                                            @<i class="fa fa-users"></i>
                                        End If
                                        @If menu.Description = "Inquiry" Then
                                            @<i class="fa fa-eye"></i>
                                        End If
                                        @If menu.Description = "Report" Then
                                            @<i class="fa fa-file-text-o"></i>
                                        End If
                                        @If menu.Description = "Master Approval" Then
                                            @<i class="fa fa-check"></i>
                                        End If
                                        <span>@menu.Description</span>
                                        @If menu.Description = "Transaction" Then
                                            @If yetReconsile > 0 Or undoneTransaction > 0 Or unreviewTransaction > 0 Then
                                                @<span class="pull-right-container">
                                                    @If CountBoth = 2 Then
                                                        @<span class="label label-danger">@CountYetAllTransaction</span>
                                                    ElseIf CountUndoneTrans = 1 Then
                                                        @<span class="label label-danger">@CountUndoneTransaction</span>
                                                    ElseIf CountReconsile = 1 Then
                                                        @<span class="label label-danger">@CountYetReconsile</span>
                                                    ElseIf CountUnapproveTransaction = 1 Then
                                                        @<span class="label label-danger">@CountUnapprovalTransaction</span>
                                                    ElseIf CountUnReview = 1 Then
                                                        @<span class="label label-danger">@CountUnreviewTransaction</span>
                                                    End If
                                                </span>
                                            End If
                                        ElseIf menu.Description = "Master" Then
                                            @If nonNPWP > 0 Then
                                                @<span class="pull-right-container">
                                                    <span class="label label-danger">@CountNonNPWP</span>
                                                </span>
                                            End If
                                        End If
                                        <i class="fa fa-angle-left pull-right"></i>
                                    </a>
                                    <ul class="treeview-menu ">
                                        @For Each submenu As Fector_Index.MsSubMenu In Fector_Index.MsUserAccess.GetSubMenus(RoleID, menu.MenuId)
                                            @<li>
                                                <a href="~/@submenu.Link">
                                                    <i class="fa fa-circle-o"></i> @submenu.Description
                                                   @If submenu.Description = "Transaction Reconcile" Then
                                                        @If yetReconsile > 0 Then
                                                            @<span class=" pull-right-container label label-danger pull-right">@yetReconsile</span>
                                                        End If
                                                    ElseIf submenu.Description = "View Transaction" Then
                                                        @If undoneTransaction > 0 Then
                                                            @<span class=" pull-right-container label label-danger pull-right">@undoneTransaction</span>
                                                        End If
                                                   ElseIf submenu.Description = "Transaction Approval" Then
                                                        @If unapprovalTransaction > 0 Then
                                                            @<span class=" pull-right-container label label-danger pull-right">@unapprovalTransaction</span>
                                                        End If
                                                   ElseIf submenu.Description = "Review Transaction Document" Then
                                                        @If unreviewTransaction > 0 Then
                                                            @<span class=" pull-right-container label label-danger pull-right">@unreviewTransaction</span>
                                                   End If
                                                    ElseIf submenu.Description = "Upload NPWP" Then
                                                        @If nonNPWP > 0 Then
                                                            @<span class=" pull-right-container label label-danger pull-right">@nonNPWP</span>
                                                        End If
                                                    End If
                                                </a>
                                            </li>
                                        Next
                                    </ul>
                                </li>
                            End If
                        Else
                            @<li class="treeview">
                                <a href="#">
                                    @If menu.Description = "Master" Then
                                        @<i class="fa fa-cogs"></i>
                                    End If
                                    @If menu.Description = "Deal" Then
                                        @<i class="fa fa-thumbs-o-up"></i>
                                    End If
                                    @If menu.Description = "User" Then
                                        @<i class="fa fa-user"></i>
                                    End If
                                    @If menu.Description = "Transaction" Then
                                        @<i class="fa fa-money"></i>
                                    End If
                                    @If menu.Description = "Download" Then
                                        @<i class="fa fa-download"></i>
                                    End If
                                    @If menu.Description = "Audit Trail" Then
                                        @<i class="fa fa-users"></i>
                                    End If
                                    @If menu.Description = "Inquiry" Then
                                        @<i class="fa fa-eye"></i>
                                    End If
                                    @If menu.Description = "Report" Then
                                        @<i class="fa fa-file-text-o"></i>
                                    End If
                                    @If menu.Description = "Master Approval" Then
                                        @<i class="fa fa-check"></i>
                                    End If
                                    <span>@menu.Description</span>
                                    @If menu.Description = "Transaction" Then
                                        @If yetReconsile > 0 Or undoneTransaction > 0 Or unreviewTransaction > 0 Then
                                            @<span class="pull-right-container">
                                                @If CountBoth = 2 Then
                                                    @<span class="label label-danger">@CountYetAllTransaction</span>
                                                ElseIf CountUndoneTrans = 1 Then
                                                    @<span class="label label-danger">@CountUndoneTransaction</span>
                                                ElseIf CountReconsile = 1 Then
                                                    @<span class="label label-danger">@CountYetReconsile</span>
                                                ElseIf CountUnapproveTransaction = 1 Then
                                                    @<span class="label label-danger">@CountUnapprovalTransaction</span>
                                                ElseIf CountUnReview = 1 Then
                                                    @<span class="label label-danger">@CountUnreviewTransaction</span>
                                                End If
                                            </span>
                                        End If
                                    ElseIf menu.Description = "Master" Then
                                        @If nonNPWP > 0 Then
                                            @<span class="pull-right-container">
                                                <span class="label label-danger">@CountNonNPWP</span>
                                            </span>
                                        End If
                                    End If
                                    <i class="fa fa-angle-left pull-right"></i>
                                </a>
                                <ul class="treeview-menu ">
                                    @For Each submenu As Fector_Index.MsSubMenu In Fector_Index.MsUserAccess.GetSubMenus(RoleID, menu.MenuId)
                                        @<li>
                                            <a href="~/@submenu.Link">
                                                <i class="fa fa-circle-o"></i> @submenu.Description
                                                @If submenu.Description = "Transaction Reconcile" Then
                                                    @If yetReconsile > 0 Then
                                                        @<span class=" pull-right-container label label-danger pull-right">@yetReconsile</span>
                                                    End If
                                                ElseIf submenu.Description = "View Transaction" Then
                                                    @If undoneTransaction > 0 Then
                                                        @<span class=" pull-right-container label label-danger pull-right">@undoneTransaction</span>
                                                    End If
                                                ElseIf submenu.Description = "Transaction Approval" Then
                                                    @If unapprovalTransaction > 0 Then
                                                        @<span class=" pull-right-container label label-danger pull-right">@unapprovalTransaction</span>
                                                    End If
                                                ElseIf submenu.Description = "Review Transaction Document" Then
                                                    @If unreviewTransaction > 0 Then
                                                        @<span class=" pull-right-container label label-danger pull-right">@unreviewTransaction</span>
                                                    End If
                                                ElseIf submenu.Description = "Upload NPWP" Then
                                                    @If nonNPWP > 0 Then
                                                        @<span class=" pull-right-container label label-danger pull-right">@nonNPWP</span>
                                                    End If
                                                End If
                                            </a>
                                        </li>
                                    Next
                                </ul>
                            </li>
                        End If
                    Next
                </ul>
            </section>
            <!-- /.sidebar -->
        </aside>
        @Code
            Dim menuRenderTime As Long = stopWatch.ElapsedMilliseconds
            Debug.WriteLine(menuRenderTime.ToString() + "ms : Menu Render Time")
            stopWatch.Restart()
        End Code
        <!-- =============================================== -->
        <!-- Content Wrapper. Contains page content -->
        <div class="content-wrapper">
            <!-- Content Header (Page header) -->
            <section class="content-header">
                <h1>
                    &nbsp; 
                </h1>
                <ol class="breadcrumb">
                    @Html.Raw(ViewBag.breadcrumb)
                </ol>
            </section>
            <!-- Main content -->
            <section class="row content">
                @RenderBody()
            </section>
            <!-- /.content -->
        </div>
        <!-- /.content-wrapper -->
        <footer class="main-footer">
            <div class="pull-right hidden-xs">
                <b>Version</b> 2.3.2
            </div>
            <strong>Copyright &copy; @Now.Year <a href="http://www.karyadigital.com">KARYA DIGITAL</a>.</strong> All rights
            reserved.
        </footer>
        <div class="control-sidebar-bg"></div>
    </div>

    @Code
        Dim bodyRenderTime As Long = stopWatch.ElapsedMilliseconds
        Debug.WriteLine(bodyRenderTime.ToString() + "ms : Body Render Time")
        stopWatch.Restart()
    End Code

    <!-- ./wrapper -->  
    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/mainjs")
    @Scripts.Render("~/bundles/jqueryui")
    @Scripts.Render("~/Scripts/multiselect.js")
    @Scripts.Render("~/AdminLTE/js")
    @Scripts.Render("~/Morris/js")
    @RenderSection("scripts", required:=False)

    <script type="text/javascript">
        $('select:not(#multiselect, #multiselect_to)').select2({  });
        var message = "@ViewBag.ErrorMessage";
        var lblUserMessage = "@TempData("DeleteUser")";
        if (message.length > 0)
            bootbox.alert(message);
        if (lblUserMessage.length > 0)
            bootbox.alert(lblUserMessage);
    </script>

    @Code
        Dim scriptRenderTime As Long = stopWatch.ElapsedMilliseconds
        Debug.WriteLine(scriptRenderTime.ToString() + "ms : Script Render Time")
        stopWatch.Stop()
        
        Dim totalTime As Long = initializationTime + styleRenderTime + menuRenderTime + bodyRenderTime + scriptRenderTime
        Debug.WriteLine("Total Time Elapsed : " + totalTime.ToString() + "ms")
    End Code

</body>
</html>
