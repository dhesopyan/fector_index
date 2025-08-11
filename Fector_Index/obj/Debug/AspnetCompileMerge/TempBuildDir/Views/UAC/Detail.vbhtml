@ModelType Fector_Index.MsUACViewModel 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "UAC", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>User Access Control</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    @Html.HiddenFor(Function(f) f.UserlevelId)
                    <label>@Html.DisplayNameFor(Function(f) f.Description)</label>
                    <div class="input-group" style="width: 100%">
                        @Html.CustomTextBoxFor(Function(f) f.Description, New With {.class = "form-control", .placeholder = "User Level Name", .style = "text-transform:uppercase"})
                        <span class="input-group-addon">@Html.CheckBoxFor(Function(f) f.FlagUseLimit) @Html.DisplayNameFor(Function(f) f.FlagUseLimit)</span>
                    </div>
                    @Html.ValidationMessageFor(Function(f) f.Description, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
             <div class="row">
                 <div class="col-md-5">
                     <select name="AvailableUACLists" id="multiselect" class="form-control" size="8" multiple="multiple" placeholder="Access List">
                         @For Each strAvailable As String In Model.AvailableUACLists
                         Dim db As New Fector_Index.FectorEntities
                         Dim sm = db.SubMenus.Find(CInt(strAvailable.trim))
                             @<option value="@sm.SubMenuId">@(sm.Menu.Description & " - " & sm.Description)</option>
                         Next
                     </select>

                 </div>
                 <div class="col-md-2">
                     <button type="button" id="multiselect_rightAll" class="btn btn-block"><i class="fa fa-forward"></i></button>
                     <button type="button" id="multiselect_rightSelected" class="btn btn-block"><i class="fa fa-chevron-right"></i></button>
                     <button type="button" id="multiselect_leftSelected" class="btn btn-block"><i class="fa fa-chevron-left"></i></button>
                     <button type="button" id="multiselect_leftAll" class="btn btn-block"><i class="fa fa-backward"></i></button>
                 </div>

                 <div class="col-md-5">
                     <select name="SelectedUACLists" id="multiselect_to" class="form-control" size="8" multiple="multiple">
                        @If Not IsNothing(Model.SelectedUACLists) Then
                        For Each strSelected As String In Model.SelectedUACLists
                        Dim db As New Fector_Index.FectorEntities
                        Dim sm = db.SubMenus.Find(CInt(strSelected.Trim))
                            @<option value="@sm.SubMenuId">@(sm.Menu.Description & " - " & sm.Description)</option>
                        Next
                        End If
                     </select>
                 </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                @If (Action = "Edit" or Action = "Create") Then
                    @<button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "UAC")"><i class="fa fa-arrow-left"></i> Back</a>
                ElseIf (Action = "Process") Then
                    @<button name="approvalButton" class="btn btn-primary" value="Approve"><i class="fa fa-check"></i> Approve</button>
                    @<button name="approvalButton" class="btn btn-primary" value="Reject"><i class="fa fa-close"></i> Reject</button>
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Approval", "UAC")"><i class="fa fa-arrow-left"></i> Back</a>
                End If
            </div>
            <div class="clearfix"></div>
        </div>
        End Using
    @Section scripts
        <script>
        $(document).ready(function(){
            $('#multiselect').multiselect();

            if ('@Action' == 'Process') {
                $("#Description").attr("readonly", "readonly");
                $("#multiselect").attr("readonly", "readonly");
                $("#FlagUseLimit").attr("readonly", "readonly");
                multiselect_rightAll.disabled = true;
                multiselect_rightSelected.disabled = true;
                multiselect_leftSelected.disabled = true;
                multiselect_leftAll.disabled = true;
            }
        });
        </script>
    End Section
</div>
