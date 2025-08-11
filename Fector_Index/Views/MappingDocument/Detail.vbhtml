@ModelType Fector_Index.MsMappingDocument 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim DocumentTransId As String = ViewContext.Controller.ValueProvider.GetValue("Id").RawValue
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "MappingDocument", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @Html.HiddenFor(Function(f) f.DocumentTransId)
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Mapping Document Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.DocumentTransId)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DocumentTransaction.Description, New With {.class = "form-control", .placeholder = "Transaction Document"})
                    @Html.ValidationMessageFor(Function(f) f.DocumentTransId, String.Empty, New With {.class = "help-block"})
                    
                </div>
            </div>
            <div class="row">
                <div class="clearfix"></div>
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter">
                                    Document LHBU ID
                                </th>
                                <th class="hidden-xs filter">
                                    Description
                                </th>
                                <th></th>
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
                                <th class="filter">
                                    Document LHBU ID
                                </th>
                                <th class="hidden-xs filter">
                                    Description
                                </th>
                                <th class="hidden-xs filter" valign="middle" align="center">
                                    
                                </th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "MappingDocument")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
    @Section scripts
        <script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTMappingDocumentDetailAjaxHandler")';

            $(document).ready(function () {
                if ('@Action' == 'Edit') {
                    $('#DocumentTransId').attr('readonly', 'readonly');
                }
            });

            var selectedUnits = [];
            function selectCheckbox(data) {
                if (selectedUnits.length < 1) {
                    selectedUnits.push(data.value);
                }
                else {
                    if (data.checked == true) {
                        if ($.inArray(data.value, selectedUnits) < 0) {
                            selectedUnits.push(data.value);
                        }
                        else {
                            selectedUnits.pop(data.value);
                        }
                    }
                    else {
                        selectedUnits.pop(data.value);
                    }

                }
            }
        </script>
    @Scripts.Render("~/Scripts/datatables/mappingdocument-detail.js")
    End Section
</div>
