' Note: For instructions on enabling IIS6 or IIS7 classic mode, 
' visit http://go.microsoft.com/?LinkId=9394802
Imports System.Web.Http
Imports System.Web.Optimization

Public Class MvcApplication
    Inherits System.Web.HttpApplication

    Sub Application_Start()
        Dim server As String = ConfigurationManager.AppSettings("Server")
        Dim db As String = ConfigurationManager.AppSettings("DB")

        AppHelper.CS = "Server=" & System.Configuration.ConfigurationManager.AppSettings("Server") & ";Database=" & System.Configuration.ConfigurationManager.AppSettings("DB") & ";User ID=userptkd; Password=ptkd"

        AreaRegistration.RegisterAllAreas()
        WebApiConfig.Register(GlobalConfiguration.Configuration)
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)
        AuthConfig.RegisterAuth()

        ModelBinders.Binders.Add(GetType(Nullable(Of Decimal)), New DecimalModelBinder())
    End Sub

    Private Sub MvcApplication_AcquireRequestState(sender As Object, e As EventArgs) Handles Me.AcquireRequestState
        Dim db As New FectorEntities

        If Not (Context.Request.Url.AbsoluteUri.ToLower.Contains("/user/logout") Or Context.Request.Url.AbsoluteUri.ToLower.Contains("/user/login") Or Context.Request.Url.PathAndQuery = "/") Then
            If Not IsNothing(HttpContext.Current.Session) AndAlso IsNothing(HttpContext.Current.Session("BranchId")) Then
                'Context.Response.Redirect("~/User/Logout")
                Context.Response.RedirectToRoute(New With {.controller = "User", .action = "Logout"})
                Return
            End If
            If Not IsNothing(HttpContext.Current.Session) AndAlso IsNothing(HttpContext.Current.Session("SessionId")) Then
                'Context.Response.Redirect("~/User/Logout")
                Context.Response.RedirectToRoute(New With {.controller = "User", .action = "Logout"})
                Return
            End If

            If Not IsNothing(HttpContext.Current.Session) AndAlso Not IsNothing(HttpContext.Current.Session("SessionId")) Then
                Dim forcePasswordChange As Boolean = False
                If Not IsNothing(HttpContext.Current.Session("HasToChangePassword")) Then
                    forcePasswordChange = HttpContext.Current.Session("HasToChangePassword")
                End If
                If forcePasswordChange And Not Context.Request.Url.AbsoluteUri.ToLower.Contains("/user/changepassword") Then
                    Context.Response.Redirect("~/User/ChangePassword")
                    Return
                End If
                If Not Fector_Index.LogUser.checkSession(User.Identity.Name, HttpContext.Current.Session("SessionId")) Then
                    'Context.Response.Redirect("~/User/Logout")
                    Context.Response.RedirectToRoute(New With {.controller = "User", .action = "Logout"})
                    Return
                End If

                ''''''''Check User Access''''''''
                Dim RequestURL As String = Request.AppRelativeCurrentExecutionFilePath
                RequestURL = RequestURL.Substring(1, RequestURL.Length - 1)

                If Not RequestURL.ToLower.Contains("ajaxhandler") And Not RequestURL.ToLower.Contains("json") And Not RequestURL.ToLower.Contains("browse") And Not RequestURL.ToLower.Contains("changepassword") And Not RequestURL.ToLower.Contains("upload/npwp") Then
                    Dim ExceptioUrl = From eurl In db.ExceptionUrl
                                   Where eurl.ExceptionUrl.Contains(RequestURL)
                                    Select eurl


                    If ExceptioUrl.Count = 0 Then
                        Dim UserLevel = (From ul In db.Users
                                        Where ul.UserId = User.Identity.Name
                                        Select ul).ToList

                        Dim UserLevelId As String = UserLevel.Item(0).UserlevelId

                        Dim AvailableMenu = (From am In db.AvailableMenu
                                             Join ua In db.UserAccesses On am.SubMenuId Equals ua.SubMenuId
                                            Where ua.UserLevelId = UserLevelId And RequestURL.Contains(am.Link)
                                            Select am).ToList

                        If AvailableMenu.Count = 0 Then
                            'HttpContext.Current.Response.Redirect("~/User/Logout")
                            Context.Response.RedirectToRoute(New With {.controller = "User", .action = "Logout"})
                            Return
                        End If
                    End If
                End If
                

                ''''''''Update Expired Time''''''''
                Fector_Index.LogUser.openPage(User.Identity.Name, HttpContext.Current.Session("SessionId"))
            End If
        End If
    End Sub
End Class
