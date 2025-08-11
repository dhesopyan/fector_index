@Code
    ViewData("Title") = "Audit Trail User"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>Audit Trail User List</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter hidden ">
                                    Log ID
                                </th>
                                <th class="filter">
                                    User ID
                                </th>
                                <th class="filter" data-id="timepicker">
                                    Login Time
                                </th>
                                <th class="filter" data-id="timepicker2">
                                    Expired Time
                                </th>
                                <th class="filter" data-id="timepicker3">
                                    Logout Time
                                </th>
                                <th class="filter hidden" >
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
                                <th class="hidden">
                                    Log ID
                                </th>
                                <th>
                                    User ID
                                </th>
                                <th>
                                    Login Time
                                </th>
                                <th>
                                    Expired Time
                                </th>
                                <th>
                                    Logout Time
                                </th>
                                <th class="hidden">
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
        var ajaxHandler = '@Url.Action("DTLogUserAjaxHandler")';
    </script>
    @Scripts.Render("~/Scripts/datatables/loguser-index.js")

    <script>
        $(document).ready(function () {
            $('#timepicker').datetimepicker({
                pickDate: true,
                pickTime: true,
                format: 'dd/MM/yyyy hh:mm:ss'
            });
            $('#timepicker2').datetimepicker({
                pickDate: true,
                pickTime: true,
                format: 'dd/MM/yyyy hh:mm:ss'
            });
            $('#timepicker3').datetimepicker({
                pickDate: true,
                pickTime: true,
                format: 'dd/MM/yyyy hh:mm:ss'
            });
        });
    </script>
End Section
