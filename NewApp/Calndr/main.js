$(function () {
    var events = [];
    var selectedEvent = null;
    FetchEventAndRenderCalendar();
    function FetchEventAndRenderCalendar() {
        events = [];

        $.ajax({
            type: "GET",
            url: 'http://localhost:52396/Home/getJSData',
            success: function (data) {
                $.each(data, function (i, v) {
                    events.push({
                        id: v.id,
                        title: v.title,
                        description: v.description,
                        start:moment(v.start),
                        end: v.end != null ? moment(v.end) : null,

                    });
                })
                
                GenerateCalender(events);
            },
            error: function (error) {
                alert('failed');
            }
        })
    }
    function GenerateCalender(events) {
        
        $('#calendar').fullCalendar('destroy');
        $('#calendar').fullCalendar({
            dayClick: function (date, jsEvent, view) {
                var name = "TaskId";
                var value = "0";
                document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(value);
                var clickDate = date.format();
                $('#title').val('');
                $('#start').val(date.format("DD-MM-YYYY"));
                $('#end').val(date.format("DD-MM-YYYY"));
                $('#description').val('');
                $('#contacts').val(null).trigger('change');
                $('#dialog').dialog('open');
                $("#error").text("");
               
                getContacts();
            },
            eventClick: function (event) {
                var name = "TaskId";
                var value = event.id.toString();
                document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(value);
                $('#start').val(event.start.format("DD-MM-YYYY HH:mm"));
                if (event.end == null) {
                $('#end').val((event.end || event.start).format("DD-MM-YYYY HH:mm"));
                } else {
                    $('#end').val(event.end.format("DD-MM-YYYY HH:mm"));
                }
              
                    $('#title').val(event.title);
                    $('#description').val(event.description);
                    $('#dialog').dialog('open');
                    $('#calendar').fullCalendar('updateEvent', event);
                    $("#error").text("");
                getContacts();
                setSelected();
                
            },
                editable: true,
                eventDrop: function(event, delta, revertFunc, jsEvent, ui, view) {
                    //var date = event.oldEvent.start.getDate() - event.start.getDate();
                    
                    if (event.end == null) {
                        endT = (event.end || event.start);
                        
                    } else {
                        endT = event.end;
                        
                    }
                    var data = {
                        TaskId: event.id,
                        title: event.title,
                        startDateT: event.start.format('YYYY-MM-DD HH:mm'),
                        endDateT: endT.format('YYYY-MM-DD HH:mm'),
                        descriptionT: event.description,

                    };
                    
                    SaveEvent(data);


                },

                events: events,
                backgroundColor: '#02ba99',
                textColor: '#fff',


            });
    }

 
        $('#dialog').dialog({
            autoOpen: false,
            width: '350px',
            show: {
                effect: 'drop',
                duration: 500
            },
            hide: {
                effect: 'clip',
                duration: 500
            }
    })

    function setSelected() {
        $('#contacts').val(null).trigger('change');
        $.ajax({
            type: "GET",
            url: 'http://localhost:52396/Home/getSelected',
            success: function (data) {
                $.each(data, function (i, v) {

                    data.push(v.ContId);
                })
               
                $('#contacts').val(data);
                $('#contacts').trigger('change');
            },
            error: function () {
                alert('Failed');
            }
        })
        
    }
    

    function getContacts() {
       

        $.ajax({
            type: "GET",
            url: 'http://localhost:52396/Home/getContList',
            success: function (data) {
                $.each(data, function (i, v) {


                    if ($('#contacts').find("option[value='" + v.ContId + "']").length) {
                       // $('#contacts').val(v.ContId).trigger('change');
                    } else {
                        // Create a DOM Option and pre-select by default
                        var newOption = new Option(v.surname, v.ContId, false, false);
                        $('#contacts').append(newOption).trigger('change');
                    } 


                   
                })
            },
            error: function () {
                alert('Failed');
            }
        })
    
    }

    $(".js-example-basic-multiple").select2({
        closeOnSelect: false,
      
        allowClear: true,
        backgroundColor:'#fff',
       
    });
      
      
    $('.datepicker').datetimepicker({
        dateFormat: 'dd-mm-yy',
        timeFormat: 'HH:mm'
    })
    

    function SaveEvent(data) {
        $.ajax({
            type: "POST",
            url: '/Home/SaveEvent',
            data: data,
            success: function (data) {
                if (data.status) {
                    //Refresh the calender
                  //  FetchEventAndRenderCalendar();
                    //$('#myModalSave').modal('hide');
                }
            },
            error: function () {
                alert('Failed');
            }
        })
    }
    $('#btnSave').click(function () {
        if ($('#title').val().trim() == "") {
            $("#error").text("Введіть назву завдання");
            return;
        }
        if ($('#start').val().trim() == "") {
            $("#error").text("Початковий час обов'язковий");
            
            return;
        }
        var des = ($('#description').val().trim()).toString();
       
        if (des.length > 3000) {
            
            $("#error").text("Опис не має перевищувати 3000 символів");
            return;
        }
        else {
          
            var startDate = moment($('#start').val(), "DD-MM-YYYY HH:mm ").toDate();
           
            if ($('#end').val().trim() != null) {
               
                var endDate = moment($('#end').val(), "DD-MM-YYYY HH:mm ").toDate();
               
                if (startDate.getYear() == endDate.getYear()) {
                    if (startDate > endDate) {
                        $("#error").text("Початковий час має бути більшим за кінцевий");
                        return;
                    }
                }
                else if (startDate.getYear() > endDate.getYear()) {
                    $("#error").text("Початковий час має бути більшим за кінцевий");
                    return;
                }
              
                
            }
        }

     
        var data = {
            TaskId: parseInt(getCookie('TaskId'),10),
            title: $('#title').val().trim(),
            startDateT: $('#start').val().trim(),
            endDateT: $('#end').val(),
            descriptionT: $('#description').val(),
        }
       SaveEvent(data);
        contList = $('.js-example-basic-multiple').val();
       
        var cont = ""; 
        if (contList != null) {
            $.each(contList, function (i, v) {
                cont = cont + v.toString() + "/";
            })
        }
        else {
            cont = "-1";
        }
         
         var contData={
           forCont: cont.toString(),
          }
       
        $('#dialog').dialog('close');
        
        SaveContacts(contData);
        FetchEventAndRenderCalendar();
 
    })
    $('#btnDelete').click(function () {
        if (parseInt(getCookie('TaskId'), 10) != 0 && confirm('Видалити це завдання?')) {
            $.ajax({
                type: "POST",
                url: '/home/DeleteEvent',
                data: { 'TaskId': parseInt(getCookie('TaskId'))},
                success: function (data) {
                    if (data.status) {
                        //Refresh the calender
                        FetchEventAndRenderCalendar();

                    }
                    $('#dialog').dialog('close');
                },
                error: function () {
                    alert('Failed');
                }
            })     
        } else if (confirm('Видалити це завдання?')){
            $('#title').val('');
            $('#start').val('');
            $('#end').val('');
            $('#description').val('');
            $('#contacts').val(null).trigger('change');
            $("#error").text("");

        }
    })
           

    function getCookie(name) {
       
        var cookie = " " + document.cookie;
        var search = " " + name + "=";
        var setStr = null;
        var offset = 0;
        var end = 0;
        if (cookie.length > 0) {
            offset = cookie.indexOf(search);
            if (offset != -1) {
                offset += search.length;
                end = cookie.indexOf(";", offset)
                if (end == -1) {
                    end = cookie.length;
                }
                setStr = unescape(cookie.substring(offset, end));
            }
        }
       
        return (setStr);
    }
    function SaveContacts(data) {
        
        $.ajax({
            type: "POST",
            url: '/Home/SaveContacts',
            data: data,
            success: function (data) {
                if (data.status) {
                    //Refresh the calender
                    FetchEventAndRenderCalendar();
                    //$('#myModalSave').modal('hide');
                }
            },
            error: function () {
                alert('Failed');
            }
        })
    }
})
