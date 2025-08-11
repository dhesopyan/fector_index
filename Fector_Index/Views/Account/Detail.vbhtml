@ModelType Fector_Index.MsAccount
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim BType As SelectList = ViewBag.BType
    Dim LHBUOption As IEnumerable(Of SelectListItem) = ViewBag.LHBUOption
    Dim Status As SelectList = ViewBag.Status
    Dim CurrencyList As SelectList = ViewBag.CurrencyList
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "Account", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData"})
    @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Account Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.AccNo)</label>
                    @Html.CustomTextBoxFor(Function(f) f.AccNo, New With {.class = "form-control", .placeholder = "Account Number", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.AccNo, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.CIF)</label>
                    @Html.CustomTextBoxFor(Function(f) f.CIF, New With {.class = "form-control", .placeholder = "CIF Number"})
                    @Html.ValidationMessageFor(Function(f) f.CIF, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.Name)</label>
                    @Html.CustomTextBoxFor(Function(f) f.Name, New With {.class = "form-control", .placeholder = "Name"})
                    @Html.ValidationMessageFor(Function(f) f.Name, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    @Html.HiddenFor(Function(f) f.CountryId)
                    <label>@Html.DisplayNameFor(Function(f) f.Country.Description)</label>
                    @Html.CustomTextBoxFor(Function(f) f.Country.Description, New With {.class = "form-control", .placeholder = "Country"})
                    @Html.ValidationMessageFor(Function(f) f.CountryId, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.BusinessTypeId)</label>
                     @Html.DropDownListFor(Function(f) f.BusinessTypeId, BType, "-- SELECT BUSINESS TYPE --", New With {.class = "form-control", .placeholder = "Business Type"})
                     @Html.ValidationMessageFor(Function(f) f.BusinessTypeId, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-3">
                     <label>Exclude from LHBU</label>
                     @Html.DropDownListFor(Function(f) f.flagNonLHBU, LHBUOption, New With {.class = "form-control", .placeholder = "LHBU"})
                     @Html.ValidationMessageFor(Function(f) f.flagNonLHBU, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.SubjectStatusId)</label>
                     @Html.DropDownListFor(Function(f) f.SubjectStatusId, Status, "-- SELECT STATUS --", New With {.class = "form-control", .placeholder = "Status", .id = "SubjectStatusId"})
                     @Html.ValidationMessageFor(Function(f) f.SubjectStatusId, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.BICode)</label>
                     <div class="input-group">
                         @Html.CustomTextBoxFor(Function(f) f.BICode, New With {.class = "form-control", .placeholder = "BI Code"})
                         <span class="input-group-btn">
                             <a href="#" class="modal-link btn btn-default" id="btnclearbicode" title="Clear"><i class='fa fa-refresh'></i></a>
                             <a href="@Url.Action("BrowseBICode", "Account")" class="modal-link btn btn-default" id="btnsearchbicode" title="Browse"><i class='fa fa-search'></i></a>
                         </span>
                     </div>
                     @Html.ValidationMessageFor(Function(f) f.BICode, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">&nbsp;</div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.TODLimitCurrency)</label>
                     @Html.DropDownListFor(Function(f) f.TODLimitcurrency, CurrencyList, New With {.class = "form-control", .placeholder = "TOD Limit Currency"})
                     @Html.ValidationMessageFor(Function(f) f.TODLimitcurrency, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.TODLimit)</label>
                     @Html.CustomTextBoxFor(Function(f) f.TODLimit, New With {.class = "form-control autoNumeric", .placeholder = "TOD Limit, 0 for No-Limit"})
                     @Html.ValidationMessageFor(Function(f) f.TODLimit, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.TOMLimitcurrency)</label>
                     @Html.DropDownListFor(Function(f) f.TOMLimitcurrency, CurrencyList, New With {.class = "form-control", .placeholder = "TOM Limit Currency"})
                     @Html.ValidationMessageFor(Function(f) f.TOMLimitcurrency, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.TOMLimit)</label>
                     @Html.CustomTextBoxFor(Function(f) f.TOMLimit, New With {.class = "form-control autoNumeric", .placeholder = "TOM Limit, 0 for No-Limit"})
                     @Html.ValidationMessageFor(Function(f) f.TOMLimit, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.SPOTLimitcurrency)</label>
                     @Html.DropDownListFor(Function(f) f.SPOTLimitcurrency, CurrencyList, New With {.class = "form-control", .placeholder = "SPOT Limit Currency"})
                     @Html.ValidationMessageFor(Function(f) f.SPOTLimitcurrency, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.SPOTLimit)</label>
                     @Html.CustomTextBoxFor(Function(f) f.SPOTLimit, New With {.class = "form-control autoNumeric", .placeholder = "SPOT Limit, 0 for No-Limit"})
                     @Html.ValidationMessageFor(Function(f) f.SPOTLimit, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.ALLLimitcurrency)</label>
                     @Html.DropDownListFor(Function(f) f.ALLLimitcurrency, CurrencyList, New With {.class = "form-control", .placeholder = "Combination Limit Currency"})
                     @Html.ValidationMessageFor(Function(f) f.ALLLimitcurrency, String.Empty, New With {.class = "help-block"})
                 </div>
                 <div class="form-group col-md-6">
                     <label>@Html.DisplayNameFor(Function(f) f.ALLLimit)</label>
                     @Html.CustomTextBoxFor(Function(f) f.ALLLimit, New With {.class = "form-control autoNumeric", .placeholder = "Combination Limit, 0 for No-Limit"})
                     @Html.ValidationMessageFor(Function(f) f.ALLLimit, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                @If (Action = "Edit") Then
                    @<button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                ElseIf (Action = "Process") Then
                    @<button name="approvalButton" class="btn btn-primary" value="Approve"><i class="fa fa-check"></i> Approve</button>
                    @<button name="approvalButton" class="btn btn-primary" value="Reject"><i class="fa fa-close"></i> Reject</button>
                End If
                @If (Action = "Process") Then
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Approval", "Account")"><i class="fa fa-arrow-left"></i> Back</a>
                Else
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "Account")"><i class="fa fa-arrow-left"></i> Back</a>
                End If
            </div>
            <div class="clearfix"></div>
        </div>
        @<style>
        .modal-content {
            position:fixed;
            left:0px;
            right:0px;
            width: 800px !important;
            height: auto !important;
            margin: auto auto !important;
        }
        </style>
        @<div id="modal-container" class="modal fade" tabindex="-1" role="dialog">
             <div class="modal-content">
             </div>
        </div>
    End Using
    @Section scripts
    <script>
    $(document).ready(function () {
        if ('@Action' == 'Edit') {
            $('#AccNo').attr('readonly', 'readonly');
            $('#CIF').attr('readonly', 'readonly');
            $('#Name').attr('readonly', 'readonly');
            $('#Country_Description').attr('readonly', 'readonly');
            $('#BICode').attr('readonly', 'readonly');
        }
        else if ('@Action' == 'Process')
        {
            $('#AccNo').attr('readonly', 'readonly');
            $('#CIF').attr('readonly', 'readonly');
            $('#Name').attr('readonly', 'readonly');
            $('#Country_Description').attr('readonly', 'readonly');
            BusinessTypeId.disabled = true;
            flagNonLHBU.disabled = true;
            SubjectStatusId.disabled = true;
            $('#BICode').attr('readonly', 'readonly');
            $('#btnclearbicode').attr('disabled', 'disabled');
            $('#btnsearchbicode').attr('disabled', 'disabled');
            $('#TODLimit').attr('readonly', 'readonly');
            $('#TOMLimit').attr('readonly', 'readonly');
            $('#SPOTLimit').attr('readonly', 'readonly');
            $('#ALLLimit').attr('readonly', 'readonly');
            TODLimitcurrency.disabled = true;
            TOMLimitcurrency.disabled = true;
            SPOTLimitcurrency.disabled = true;
            ALLLimitcurrency.disabled = true;
            btnsearchbicode.disabled = true;
        }
    });

    $(function () {
        $('body').on('click', '#btnsearchbicode', function (e) {
            e.preventDefault();
            var link = $(this).attr('href');
            var name = $('#Name').val();
            var status = $('#SubjectStatusId').val();
            if (!status) {
                bootbox.alert("Please select status first");
            }
            else {
                name = encodeURIComponent(name);
                status = encodeURIComponent(status);
                link = link + '?name=' + name + '&selectedstatus=' + status;
                $(this).attr('href', link);
                $(this).attr('data-target', '#modal-container');
                $(this).attr('data-toggle', 'modal');
            }
        });
        $('body').on('click', '#btnclearbicode', function (e) {
            e.preventDefault();
            $('#BICode').val('');
        });
        $('#modal-container').on('hidden.bs.modal', function () {
            $(this).removeData('bs.modal');
            $('#btnsearchbicode').attr('href', '@Url.Action("BrowseBICode", "Account")');
        });
        $('#SubjectStatusId').change(function () {
            $("#BICode").val("");
        })
    });
    </script>
    End Section
</div>
