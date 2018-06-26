function ajaxLike(questionId = 1, answerId = 1, likeOrDis = 1) {
    var likeLabel;
    var dislikeLabel;
    var likeNum;
    var dislikeNum;
    if (questionId !== 1) {
        likeLabel = $("#question_no_" + questionId + " .question_likes_label span");
        dislikeLabel = $("#question_no_" + questionId + " .question_dislikes_label span");
        likeNum = document.getElementById("question_no_" + questionId).getElementsByClassName("question_likes_count")[0];
        dislikeNum = document.getElementById("question_no_" + questionId).getElementsByClassName("question_dislikes_count")[0];
    } else {
        likeLabel = $("#answer_no_" + answerId + " .answer_likes_label span");
        dislikeLabel = $("#answer_no_" + answerId + " .answer_dislikes_label span");
        likeNum = document.getElementById("answer_no_" + questionId).getElementsByClassName("question_likes_count")[0];
        dislikeNum = document.getElementById("answer_no_" + questionId).getElementsByClassName("question_dislikes_count")[0];
//        likeNum = $("#alikenum_ " + answerId);
//        dislikeNum = $("#adislikenum_" + answerId);
    }
    if (likeOrDis === 1) {
        if (dislikeLabel.hasClass("active")) {
            dislikeLabel.removeClass("active");
            dislikeNum.innerHTML = parseInt(dislikeNum.innerHTML) - 1;
        }
        if (likeLabel.hasClass("active")) {
            likeLabel.removeClass("active");
            likeNum.innerHTML = parseInt(likeNum.innerHTML )- 1;
        } else {
            likeLabel.addClass("active");
            likeNum.innerHTML = parseInt(likeNum.innerHTML) + 1;
        }
    } else {
        if (likeLabel.hasClass("active")) {
            likeLabel.removeClass("active");
            likeNum.innerHTML = parseInt(likeNum.innerHTML) - 1;
        }
        if (dislikeLabel.hasClass("active")) {
            dislikeLabel.removeClass("active");
            dislikeNum.innerHTML = parseInt(dislikeNum.innerHTML) - 1;
        } else {
            dislikeLabel.addClass("active");
            dislikeNum.innerHTML = parseInt(dislikeNum.innerHTML) + 1;
        }
    }
    var xHttp = new XMLHttpRequest();
    xHttp.open("post", "/Home/Like?questionId=" + questionId + "&answerId=" + answerId + "&like_or_dis=" + likeOrDis, true);
    xHttp.send();

}

function ajaxDivisionSelect(division) {
    $.ajax({
        type: "GET",
        url: "/Home/GetDistricts?division=" + division,
        success: function(data) {
            for (var i = 0; i < data.length; i++) {
                $("#district_selector").append(new Option(data[i], data[i]));
            }
        }
    });
}