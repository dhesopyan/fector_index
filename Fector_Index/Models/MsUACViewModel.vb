Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsUACViewModel
    Public Property UserlevelId As Integer

    <DisplayName("User Level Name")> _
    <Required(ErrorMessage:="Please fill user level name")> _
    Public Property Description As String

    <DisplayName("Use Limit")> _
    Public Property FlagUseLimit As Boolean

    <DisplayName("Access Granted")> _
    Public Property AvailableUACLists As String()
    Public Property SelectedUACLists As String()
    Public Property UAC As List(Of MsUserAccess)

    Public Shared Function GetAllActiveMenu(ByVal db As FectorEntities) As List(Of MsMenu)
        Dim menu = From mn In db.Menus
                      Where mn.FlagActive = True
                      Order By mn.Ordering
                      Select mn

        Return menu.ToList
    End Function

    Public Shared Function GetAllActiveSubmenuInMenu(ByVal db As FectorEntities, ByVal menuId As Integer) As List(Of MsSubMenu)
        Dim submenu = From sm In db.SubMenus
                   Where sm.FlagActive = True And sm.MenuId = menuId
                   Order By sm.Ordering
                   Select sm

        Return submenu.ToList
    End Function

    Public Shared Function GetAvailableActiveSubmenuInMenu(ByVal db As FectorEntities, ByVal userlevel As Integer) As List(Of MsSubMenu)
        Dim submenu = (From sm In db.SubMenus.Include("Menu")
                       Where sm.FlagActive = True And Not (db.UserAccesses.Where(Function(f) f.UserLevelId = userlevel).Any(Function(f) f.SubMenuId = sm.SubMenuId))
                       Order By sm.Menu.Ordering, sm.Ordering
                       Select sm).ToList

        Return submenu
    End Function

    Public Shared Function GetAllActiveSubmenu(ByVal db As FectorEntities) As List(Of MenuAvailable)
        Dim submenu = From sm In db.SubMenus
                      Join mn In db.Menus On sm.MenuId Equals mn.MenuId
                      Where sm.FlagActive = True And mn.FlagActive = True
                      Order By mn.Ordering, sm.Ordering
                      Select New MenuAvailable With {.SubMenuID = sm.SubMenuId, .SubMenuName = sm.Description, .MenuGroup = mn.Description}

        Return submenu.ToList
    End Function

    Public Shared Function GetUAC(ByVal UserLevelId As Integer, ByVal db As FectorEntities) As MsUACViewModel
        Dim UserLevel = db.UserLevels.Where(Function(f) f.UserlevelId = UserLevelId).SingleOrDefault
        Dim UAC = (From u In db.UserAccesses
                       Where u.UserLevelId = UserLevelId
                       Select u).ToList

        Dim mdl As New MsUACViewModel
        With mdl
            .UserlevelId = UserLevel.UserlevelId
            .Description = UserLevel.Description
            .FlagUseLimit = UserLevel.FlagUseLimit
            .UAC = UAC
            Dim strUAC(UAC.Count) As String
            Dim i As Integer = 0
            For Each u As MsUserAccess In UAC
                strUAC(i) = u.SubMenuId
                i += 1
            Next
            mdl.SelectedUACLists = strUAC
        End With

        Return mdl
    End Function
End Class

Public Class MenuAvailable
    Public Property SubMenuID As Integer
    Public Property SubMenuName As String
    Public Property MenuGroup As String
End Class
