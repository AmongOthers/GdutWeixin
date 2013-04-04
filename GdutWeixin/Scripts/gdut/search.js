$(function () {
	//选择器
	var $resultList = $(".resultList");
	var onSearchDone = function (books, pageCount) {
		$(booksTemplate({ books: books })).appendTo($resultList);
		$resultList.listview("refresh");
		$(".resultList a[data-role='button']").button();
	};

	var onSearchError = function (error) {
	};

	var onAjaxError = function (status, error) {
	};

	var booksTemplate = getBooksTemplate();
	var keyword = getURLParameter("keyword");
	var page = getURLParameter("page");
	var pageSize = getURLParameter("pageSize");
	//如果url提供了keyword和page参数，则进行一次初始查询
	if (keyword !== null) {
		$(".info").show();
		$(".search").hide();
		$(".info label").text(keyword);
		if (pageSize === null) {
			pageSize = 50;
		}
		if (page === null) {
			page = 1;
		}
		search(keyword, pageSize, page, onSearchDone, onSearchError, onAjaxError);
	}
	else {
		$(".info").hide();
		$(".search").show();
	}
	$(".info input").on("focus", function () {
		$(".info").hide();
		$(".search").show();
	});
	var $searchBtn = $(".search .btn");
	$searchBtn.on("click", function () {
		$(".info").show();
		$(".search").hide();
	});
	//由于中文输入法不会触发keyup事件，所以需要写一个定时器检测
	var $searchInput = $(".search input[type=text]");
	var i;
	$searchInput.on("focus", function () {
		i = setInterval(function () {
			var text = $searchInput.val();
			if (text.length > 0) {
				$(".ui-btn-text", $searchBtn).text("搜索");
			}
			else {
				$(".ui-btn-text", $searchBtn).text("取消");
			}
		}, 50);
	});

	$searchInput.on("blur", function () {
		clearInterval(i);
	});

	//加入收藏
	$resultList.on("click", "div.bookFavoriate", function () {
		if ($(this).hasClass("notInFavoriate")) {
			$(this).removeClass("notInFavoriate");
			$(this).addClass("inFavoriate");
		}
	});
});

var getBooksTemplate = function () {
	return Handlebars.compile($("#books").html());
};

//搜索图书
var search = function (keyword, pageSize, page, onSearchDone, onSearchError, onAjaxError) {
	$.ajax("Library/Search", { data: { keyword: keyword, page: page, pageSize: pageSize } }).done(function (data, textStatus, jqXHR) {
		if (data.Error === null) {
			onSearchDone(data.Books, data.PageCount);
		}
		else {
			onSearchError(data.Error);
		}
	}).error(function (jqXHR, textStatus, errorThrown) {
		onAjaxError(textStatus, errorThrown);
	});
};

//获取url参数
var getURLParameter = function (name) {
	return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null;
};