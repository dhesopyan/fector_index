@Code
    ViewData("Title") = "User"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim ErrorMessage As String = TempData ("Delete")
End Code
<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action="Index","User List","User Approval List")</h4>
            </div>
            <div class="pull-right">
                @If Action = "Index" Then
                    @<a href="~/User/Create" Class="btn btn-primary"><i class="fa fa-plus"></i>&nbsp;Create</a>    
                End If
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12 ">
                    <span class="field-validation-valid" data-valmsg-for="lblError" data-valmsg-replace="true"></span>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter">
                                    Username
                                </th>
                                <th class="hidden-xs filter">
                                    Full Name
                                </th>
                                <th class="filter">
                                    Level
                                </th>
                                <th class="filter">
                                    Branch
                                </th>
                                <th class="filter">
                                    Limit
                                </th>
                                <th class="filter" id="filterDDL">
                                    Status
                                </th>
                                <th>
                                    <a id="BtnFilter" class="btn btn-primary btn-sm">
                                        <span class="fa fa-search"></span>
                                    </a>
                                    <a id="BtnClear" class="btn btn-primary btn-sm">
                                        <span class="fa fa-refresh"></span>
                                    </a>
                                </th>
                            </tr>
                            <tr id="GvHeader" class="GvHeader">
                                <th>
                                    Username
                                </th>
                                <th class="hidden-xs">
                                    Full Name
                                </th>
                                <th>
                                    Level
                                </th>
                                <th>
                                    Branch
                                </th>
                                <th>
                                    Limit
                                </th>
                                <th>
                                    Status
                                </th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
<style>
    .dataTables_filter, .dataTables_info {
        display: none;
    }
</style>
@Section scripts
    <script>
        var baseUrl = '@Url.Content("~")';
    </script>
    @If Action = "Index" Then
        @<script>
            var ajaxHandler = '@Url.Action("DTUserAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/user-index.js")
    ElseIf Action = "Approval" Then
        @<script>
            var ajaxHandler = '@Url.Action("DTUserApprovalAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/user-approval.js")
    End If
End Section
