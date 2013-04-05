$(function () {
	var hasSearched = false;
	var currentResultCount = 0, resultCount = -1, pageCount = -1, results = [];
	var pageSize = Number(getURLParameter("pageSize"));
	if (!pageSize) {
		pageSize = 20;
	}
	var page = Number(getURLParameter("page"));
	if (!page) {
		page = 0;
	}
	var keyword = getURLParameter("keyword");

	//点击info的输入框到搜索面板
	var $infoSearchInput = $(".info input");
	$infoSearchInput.on("focus", function () {
		showSearch();
	});
	//搜索图书并控制"更多"和"加载中"的按钮的样式
	var $moreBtn = $(".moreBtn");
	var onLoading = function () {
		$moreBtn.addClass("loading ui-disabled");
		$infoSearchInput.addClass("ui-disabled");
	};
	var onLoadingFinished = function () {
		$moreBtn.removeClass("loading ui-disabled");
		$infoSearchInput.removeClass("ui-disabled");
		$moreBtn.addClass("notLoading");
	};
	var search = function () {
		hasSearched = true;
		if (results.length - currentResultCount >= pageSize ||
			(results.length == resultCount && resultCount != -1)) {
			var books = results.slice(currentResultCount, currentResultCount + pageSize);
			onSearchDone(books, pageCount, resultCount);
		}
		//获取下一页
		else {
			onLoading();
			page++;
			_search(keyword, page, function (books, pageCount, resultCount) {
				if (books != null) {
					results = results.concat(books);
					onSearchDone(results.slice(currentResultCount, currentResultCount + pageSize), pageCount, resultCount);
				}
				else {
					onSearchDone(null, pageCount, resultCount);
				}
				onLoadingFinished();
			}, function (error) {
				onSearchError(error);
				onLoadingFinished();
			}, function (status, error) {
				onAjaxError(status, error);
				onLoadingFinished();
			});
		}
	};
	$moreBtn.on("click", search);

	var $resultList = $(".resultList");
	var $resultCount = $(".resultCount");
	var booksTemplate = getBooksTemplate();
	var $title = $("div.info label");
	var onSearchDone = function (books, _pageCount, _resultCount) {
		pageCount = _pageCount;
		resultCount = _resultCount;
		currentResultCount += books == null ? 0 : books.length;
		if (currentResultCount == resultCount) {
			$moreBtn.hide();
		}
		else {
			$moreBtn.show();
		}
		if (!$resultCount.visible) {
			$resultCount.show();
		}
		$resultCount.text("搜索结果： " + currentResultCount + " / " + resultCount);
		if (books != null) {
			for (var i = 0; i < books.length; i++) {
				var book = books[i];
				book.IsInFavoriate = localStorage.getItem(book.Index) !== null;
			}
			$(booksTemplate({ books: books })).appendTo($resultList);
			$resultList.listview("refresh");
			$(".resultList a[data-role='button']").button();
		}
	};

	var onSearchError = function (error) {
		alert(error);
	};

	var onAjaxError = function (status, error) {
		alert(status + error);
	};

	var $page = $("div[data-role='page']");
	var showInfo = function () {
		$page.addClass("bookInfo");
		$page.removeClass("bookSearch");
	};

	var showSearch = function () {
		$page.addClass("bookSearch");
		$page.removeClass("bookInfo");
	};

	//如果url提供了keyword， 则自动进行搜索
	var $searchBtn = $(".search .btn");
	if (keyword !== null) {
		showInfo();
		$(".info label").text(keyword);
		search();
	}
	else {
		$(".ui-btn-text", $searchBtn).text("搜索");
		showSearch();
	}
	//搜索页面点击搜索按钮
	var isSearchBtn = true;
	var $searchUiInput = $(".search .ui-input-search");
	var $searchInput = $(".search input[type=text]");
	$searchBtn.on("click", function () {
		if (!isSearchBtn) {
			//如果曾经进行过搜索，就会回到结果页面
			if (hasSearched) {
				showInfo();
			}
		}
		else {
			keyword = $searchInput.val();
			if (!keyword) {
				$searchUiInput.addClass("error");
				return;
			}
			currentResultCount = 0, resultCount = -1, pageCount = -1, results = [];
			page = 1;
			pageSize = 20;
			$(".resultList li").remove();
			$(".info label").text(keyword);
			$(".resultCount").hide();
			showInfo();
			search();
		}
	});
	//由于中文输入法不会触发keyup事件，所以需要写一个定时器检测
	var i;
	$searchInput.on("focus", function () {
		$searchUiInput.removeClass("error");
		i = setInterval(function () {
			var text = $searchInput.val();
			if (text.length > 0) {
				isSearchBtn = true;
				$(".ui-btn-text", $searchBtn).text("搜索");
			}
			else {
				if (hasSearched) {
					isSearchBtn = false;
					$(".ui-btn-text", $searchBtn).text("取消");
				}
			}
		}, 50);
	});

	$searchInput.on("blur", function () {
		clearInterval(i);
	});

	//加入收藏和移除收藏
	var $addTip = $(".addIntoFavoriate");
	var $removeTip = $(".removeFromFavoriate");
	$resultList.on("click", "div.bookRight", function () {
		if ($(this).hasClass("notInFavoriate")) {
			$(this).removeClass("notInFavoriate");
			$(this).addClass("inFavoriate");
			localStorage.setItem($(this).data("index"), "");
			$addTip.popup("open", { positionTo: $(this) });
			setTimeout(function () {
				$addTip.popup("close");
			}, 500);
		}
		else {
			$(this).removeClass("inFavoriate");
			$(this).addClass("notInFavoriate");
			localStorage.removeItem($(this).data("index"));
			$removeTip.popup("open", { positionTo: $(this) });
			setTimeout(function () {
				$removeTip.popup("close");
			}, 500);
		}
	});
});

var getBooksTemplate = function () {
	return Handlebars.compile($("#books").html());
};

var _search = function (keyword, page, onSearchDone, onSearchError, onAjaxError) {
	//图书馆查询页数最大值
	var MAX_PAGESIZE = 50;
	$.ajax("Library/Search", { data: { keyword: keyword, page: page, pageSize: MAX_PAGESIZE } }).done(function (data, textStatus, jqXHR) {
		if (data.Error === null) {
			onSearchDone(data.Books, data.PageCount, data.ResultCount);
		}
		else {
			onSearchError(data.Error);
		}
	}).error(function (jqXHR, textStatus, errorThrown) {
		onAjaxError(textStatus, errorThrown);
	});
}

//获取url参数
var getURLParameter = function (name) {
	return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null;
};