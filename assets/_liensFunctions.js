
function topBarChatNotif(entete, resume, datetime) {
    var content = '<div class="media" id = "media" >'
        + '<div class="avatar-xs me-3">'
        + '<span class="avatar-title bg-primary rounded-circle font-size-16">'
        + '<i class="mdi mdi-chat"></i>'
        + '</span>'
        + '</div>'
        + '<div class="media-body">'
        + '<h6 class="mt-0 mb-1">' + entete + '</h6>'
        + '<div class="font-size-12 text-muted">'
        < +'p class="mb-1">' + resume + '</p>'
        + '<p class="mb-0"><i class="mdi mdi-clock-outline"></i> ' + datetime + '</p>'
        + '</div>'
        + '</div>'
        + '</div >';
    alert(content)
    return content;
}