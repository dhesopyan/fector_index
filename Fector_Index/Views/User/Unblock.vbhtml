@Code
    ViewData("Title") = "User"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>User Unblock</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter">
                                    Username
                                </th>
                                <th class="filter">
                                    Full Name
                                </th>
                                <th class="filter">
                                    Level
                                </th>
                                <th class="filter">
                                    Branch
                                </th>
                                <th class="filter">
                                    Login Attempt
                                </th>
                                <th>
                                    <a id="BtnFilter" class="btn btn-default btn-sm">
                                        <span class="fa fa-search"></span>
                                    </a>
                                    <a id="BtnClear" class="btn btn-default btn-sm">
                                        <span class="fa fa-refresh"></span>
                                    </a>
                                </th>
                            </tr>
                            <tr id="GvHeader" class="GvHeader">
                                <th>
                                    Username
                                </th>
                                <th>
                                    Full Name
                                </th>
                                <th>
                                    Level
                                </th>
                                <th>
                                    Branch
                                </th>
                                <th>
                                    Login Attempt
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
        var ajaxHandler = '@Url.Action("DTUserUnblockAjaxHandler")';
    </script>
    @Scripts.Render("~/Scripts/datatables/user-unblock.js")
End Section
