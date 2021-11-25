var Tables;
(function (Tables) {
    //
    // Features of a Basic table:
    //   - Row selection and Select All. If a row selection checkbox exists then a
    //     select all checkbox is added to the column header
    //  - Delete button enable disable based on whether at least one row is select. 
    //  - Delete button submission, even when a confirmation dialog is used.
    //  - Row selection is preserved when clicking a sort link
    //  - Row selection is preserved when clicking a details navigation link
    //
    var Basic = /** @class */ (function () {
        //
        // Constructor
        //  - Pass a jquery selector that identifies tables that should 
        //    be enhanced.
        //
        function Basic(selector) {
            var _this = this;
            $(function () {
                _this.$table = $(selector);
                _this.Configure();
            });
        }
        //
        // Configure event binding etc for the table
        //
        Basic.prototype.Configure = function () {
            var _this = this;
            //
            // Handle each table (usually it's only one)
            //
            this.$table.each(function (idx, table) {
                //
                // Check if the first row of the table has at least one checkbox
                //
                var CellWithCheckbox = $('tbody>tr:first-child input[type=checkbox]:first', table);
                var CellWithRadio = $('tbody>tr:first-child input[type=radio]:first', table);
                var UsesSelection = (CellWithCheckbox.length !== 0) || (CellWithRadio.length != 0);
                var UsesSecondaryRow = $('tbody>tr:first-child', table).hasClass('HasSecondaryRow');
                //
                // Add features if there is row selection
                //
                if (UsesSelection) {
                    _this.ConfigureDeleteButton();
                    _this.ConfigureSelection(table);
                    _this.ConfigureSelectionPersistance(table);
                    if (UsesSecondaryRow) {
                        //$(table).RowHighlight({ SingleRows: false });
                    }
                }
                if ($(table).hasClass('stickyheaders')) {
                    $(table).attr({ 'cellspacing': '0' });
                }
                //
                // Initialize the state of the UI
                //
                _this.SetUi();
                //
                // If any rows started selected raise the event
                //
                var FirstSelectedRow = $('tbody input[type=checkbox]:checked, tbody input[type=radio]:checked', table).first();
                if (FirstSelectedRow.length !== 0) {
                    /*if (FirstSelectedRow.offset().top > window.innerHeight) {
                        console.log(FirstSelectedRow.offset().top, window.innerHeight);
                        window.scrollTo(window.scrollX, FirstSelectedRow.offset().top - 100);
                    }*/
                    $(table).trigger('rowselected');
                }
            });
        };
        Basic.prototype.ConfigureColumnResizing = function (table) {
            $('thead tr th:not(:last-child)', table).append($('<div></div>').addClass('resizer').append('<div></div>'));
        };
        //
        // Add a select all checkbox and bind an event for row selection
        // such that unchecking a row will uncheck the select all checkbox.
        //
        Basic.prototype.ConfigureSelection = function (table) {
            var _this = this;
            //
            // Bind row selection checkboxes and clicking on row
            //
            $(table).on('click', 'thead input[type=checkbox]', function (e) { return _this.OnClickSelectAll(e); });
            $(table).on('click', 'tbody input[type=checkbox]', function (e) { return _this.OnClickCheckbox(e); });
            $(table).on('click', 'tbody input[type=radio]', function (e) { return _this.OnClickCheckbox(e); });
            $('> tbody > tr', table).on('click', function (e) { return _this.OnClickRow(e); });
        };
        //
        // Bind the delete button (might be in a modal dialog if confirmation was required)
        //
        Basic.prototype.ConfigureDeleteButton = function () {
            if ($('#cmdDelete').data('bs-toggle') !== 'modal') {
                $('#cmdDelete').click(function () {
                    $('#cmdDelete').closest('form').submit();
                });
            }
            else {
                $($('#cmdDelete').data('bs-target') + ' .btn-danger').click(function (e) {
                    (new window.bootstrap.Modal($('#ConfirmDelete')[0])).hide();
                    $('main .table').closest('form').submit();
                });
            }
        };
        //
        // Bind events to links that navigate away from this page
        //
        Basic.prototype.ConfigureSelectionPersistance = function (table) {
            var _this = this;
            //
            // These are links inside the table
            //
            $(table).on('click', 'thead a', function (e) { return _this.OnClickNavigationLink(e, table); });
            $(table).on('click', 'tbody a', function (e) { return _this.OnClickNavigationLink(e, table); });
            //
            // These are standard links outside the table
            //
            $('#cmdNew').on('click', function (e) { return _this.OnClickNavigationLink(e, table); });
            $('#PageSizeSelection a').on('click', function (e) { return _this.OnClickNavigationLink(e, table); });
        };
        //
        // When a checkbox is clicked raise the rowselected event
        //
        Basic.prototype.OnClickCheckbox = function (e) {
            this.SetUi();
            $(e.target).closest('table').trigger('rowselected');
        };
        //
        // When a row is click also click the selection checkbox
        //  - unless the click occurred on the selection checkbox
        //    or a link
        //
        Basic.prototype.OnClickRow = function (e) {
            var target = $(e.target);
            if (!(target.is('input[type=checkbox]') || target.is('input[type=radio]'))
                && !target.is('A')
                && !target.hasClass('form-check-label')) {
                var Row = $(e.currentTarget); //target.closest('TR');
                var Checkbox = $('input[type=checkbox]', Row);
                if (Checkbox.length != 0) {
                    Checkbox.prop('checked', !Checkbox.prop('checked'));
                }
                else {
                    $('input[type=radio]', Row).prop('checked', true);
                }
                this.SetUi();
                $(e.target).closest('table').trigger('rowselected');
            }
        };
        //
        // When a navigation link is click add the list of selected rows
        // to the link's address. If the link has a ReturnToUrl parameter
        // assume it is the last parameter and correctly escape the 
        // selected row ids parameter. If the link does not have a ReturnToUrl
        // parameter add the selected row ids parameter as a regular parameter.
        // 
        Basic.prototype.OnClickNavigationLink = function (e, table) {
            //
            // Pull out the query string parameters from the link
            //
            var Link = $(e.target);
            var LinkUrl = Link.attr('href');
            var QuestionMarkPosition = LinkUrl.indexOf('?');
            var LinkQueryString = '';
            if (QuestionMarkPosition !== -1) {
                LinkQueryString = LinkUrl.substr(QuestionMarkPosition + 1);
                LinkUrl = LinkUrl.substr(0, QuestionMarkPosition);
            }
            var Parameters = LinkQueryString.split('&');
            //
            // If there already is a SelectedRowIds parameter remove it
            //
            var ParameterToRemove = -1;
            for (var i = 0; i < Parameters.length; i += 1) {
                var KeyValue = Parameters[i].split('=');
                if (KeyValue[0] === 'SelectedRowIds') {
                    ParameterToRemove = i;
                }
            }
            if (ParameterToRemove !== -1) {
                Parameters = Parameters.slice(ParameterToRemove, 1);
            }
            //
            // Figure out if there is a ReturnToUrl parameter 
            //
            var ReturnToUrl = -1;
            for (var i = 0; i < Parameters.length; i += 1) {
                var KeyValue = Parameters[i].split('=');
                if (KeyValue[0] === 'ReturnToUrl') {
                    ReturnToUrl = i;
                    break;
                }
            }
            //
            // Get the new parameter to be added to the query string
            //
            var SelectionParameters = this.GetSelectedItemsListParameter(table);
            //
            // If there's a return to url just add the new parameter to its value
            //
            if (ReturnToUrl !== -1) {
                Parameters[ReturnToUrl] += encodeURIComponent("&" + SelectionParameters);
            }
            else {
                //
                // Otherwise add it as a new parameter
                //
                Parameters.push(SelectionParameters);
            }
            //
            // Put the parameters back together
            //
            var UpdatedQueryString = '';
            for (var i = 0; i < Parameters.length; i += 1) {
                if (Parameters[i] !== '') {
                    if (i != 0) {
                        UpdatedQueryString += '&';
                    }
                    UpdatedQueryString += Parameters[i];
                }
            }
            //
            // Replace the old href with the updated one
            //
            Link.attr('href', LinkUrl + '?' + UpdatedQueryString);
        };
        //
        // Get the list of selected rows
        //
        Basic.prototype.GetSelectedItemsListParameter = function (table) {
            //
            // If no rows are selected then there is no querystring parameter
            //
            var AllCheckboxes = $('tbody input[type=checkbox]:checked', table);
            if (AllCheckboxes.length === 0) {
                return '';
            }
            //
            // Build a comma separated list of selected values
            //
            var Items = '';
            var First = true;
            AllCheckboxes.each(function (idx, elm) {
                if (!First) {
                    Items += ',';
                }
                Items += $(elm).attr('value');
                First = false;
            });
            //
            // Return the querystring parameter
            //  - The name of the parameter is the same as the name of the checkboxes
            //
            return AllCheckboxes.first().attr('name') + '=' + encodeURIComponent(Items);
        };
        //
        // Set the UI based on its state
        //  - Select all checkbox
        //  - Delete button
        //
        Basic.prototype.SetUi = function () {
            this.$table.each(function (idx, table) {
                //
                // Get lists of all the checkboxes and just the checked checkboxes
                //
                var AllCheckboxes = $('tbody input[type=checkbox], tbody input[type=radio]', table);
                var OnlyChecked = $('tbody input[type=checkbox]:checked, tbody input[type=radio]:checked', table);
                //
                // If all checkboxes are checked then show the select all checkbox as checked
                //
                $('thead input[type=checkbox]').prop('checked', AllCheckboxes.length !== 0 && OnlyChecked.length === AllCheckboxes.length);
                //
                // If at least one checkbox is checked enable the delete button
                //
                $('#cmdDelete').prop('disabled', OnlyChecked.length === 0);
                $('button[data-onselection=\'enable\']').prop('disabled', OnlyChecked.length === 0);
                $('a.btn[data-onselection=\'enable\']').prop('disabled', OnlyChecked.length === 0);
                $('button[data-onselections=\'enable\']').prop('disabled', OnlyChecked.length <= 1);
                $('a.btn[data-onselections=\'enable\']').prop('disabled', OnlyChecked.length > 1);
                //
                // Add a "selected" class to each row that is selected
                //
                AllCheckboxes.each(function (index, element) {
                    var Row = $(element).closest('TR');
                    var IsChecked = $(element).is(':checked');
                    var HasSelected = Row.hasClass('active');
                    if (IsChecked) {
                        if (!HasSelected) {
                            Row.addClass('active');
                        }
                    }
                    else {
                        if (HasSelected) {
                            Row.removeClass('active');
                        }
                    }
                });
            });
        };
        //
        // If the SelectAll checkbox was clicked set the row selection checkboxes 
        //
        Basic.prototype.OnClickSelectAll = function (e) {
            var checked = e.target.checked;
            $(e.target).closest('TABLE').find('TBODY input').each(function (idx, elm) {
                elm.checked = checked;
            });
            this.SetUi();
            $(e.target).closest('table').trigger('rowselected');
        };
        return Basic;
    }());
    Tables.Basic = Basic;
})(Tables || (Tables = {}));
//# sourceMappingURL=BasicTable.js.map