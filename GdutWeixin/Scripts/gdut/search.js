/*
索引作为收藏项目放进本地存储的时候，会加上前缀"@"作为标志
*/
var onSearchPageInit = function () {
	var hasSearched = false;
	var currentResultCount = 0, resultCount = -1, pageCount = -1, results = [];
	var i;
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
				(results.length === resultCount && resultCount !== -1)) {
			var books = results.slice(currentResultCount, currentResultCount + pageSize);
			onSearchDone(books, pageCount, resultCount);
		} else {
			onLoading();
			page++;
			librarySearch(keyword, page, function (books, pageCount, resultCount) {
				if (books !== null) {
					results = results.concat(books);
					onSearchDone(results.slice(currentResultCount, currentResultCount + pageSize),
						pageCount, resultCount);
				} else {
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
	var $bookBanner = $(".bookBanner");
	var $back2To = $(".back2Top");
	$back2To.on("click", function () {
		$.mobile.silentScroll(0);
	});
	var $resultCount = $(".resultCount");
	var booksTemplate = getBooksTemplate();
	var $title = $("div.info label");
	var onSearchDone = function (books, pPageCount, pResultCount) {
		pageCount = pPageCount;
		resultCount = pResultCount;
		currentResultCount += books === null ? 0 : books.length;
		if (currentResultCount === resultCount) {
			$moreBtn.hide();
		} else {
			$moreBtn.show();
		}
		if (!$bookBanner.visible) {
			$bookBanner.show();
		}
		$resultCount.text("搜索结果： " + currentResultCount + " / " + resultCount);
		if (books !== null) {
			for (i = 0; i < books.length; i++) {
				var book = books[i];
				book.LocalKey = "@" + book.Index;
				book.IsInFavoriate = localStorage.getItem(book.LocalKey) !== null;
				book.BookJson = JSON.stringify(book);
			}
			$(booksTemplate(books)).appendTo($resultList);
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
		$page.removeClass("bookInit");
	};

	var showSearch = function () {
		$page.addClass("bookSearch");
		$page.removeClass("bookInfo");
		$page.removeClass("bookInit");
	};

	var showInit = function () {
		$page.addClass("bookInit");
	};

	var isSearchBtn = true;
	//如果url提供了keyword， 则自动进行搜索
	var $searchBtn = $(".search .btn");
	if (keyword !== null) {
		hasSearched = true;
		isSearchBtn = false;
		showInfo();
		$(".info label").text(keyword);
		search();
	} else {
		$(".ui-btn-text", $searchBtn).text("搜索");
		showInit();
	}

	var $keywordLabel = $(".info label");
	//搜索页面点击搜索按钮
	var $searchUiInput = $(".ui-input-search");
	var $searchInput = $("input[data-type='search']");
	$searchBtn.on("click", function () {
		if (!isSearchBtn) {
			//如果曾经进行过搜索，就会回到结果页面
			if (hasSearched) {
				showInfo();
			}
		} else {
			keyword = $searchInput.val();
			if (!keyword || !(keyword.trim())) {
				$searchUiInput.addClass("error");
				return;
			}
			currentResultCount = 0;
			resultCount = -1;
			pageCount = -1;
			results = [];
			page = 1;
			pageSize = 20;
			$(".resultList li").remove();
			$keywordLabel.text(keyword);
			$bookBanner.hide();
			showInfo();
			search();
		}
	});
	//由于中文输入法不会触发keyup事件，所以需要写一个定时器检测
	$searchInput.on("focus", function () {
		$searchUiInput.removeClass("error");
		i = setInterval(function () {
			var text = $searchInput.val();
			if (text.length > 0) {
				isSearchBtn = true;
				$(".ui-btn-text", $searchBtn).text("搜索");
			} else {
				if (hasSearched) {
					isSearchBtn = false;
					$(".ui-btn-text", $searchBtn).text("取消");
				}
			}
		}, 200);
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
			localStorage.setItem($(this).data("key"), JSON.stringify($(this).data("json")));
			$addTip.popup("open", { positionTo: $(this) });
			setTimeout(function () {
				$addTip.popup("close");
			}, 500);
		} else {
			$(this).removeClass("inFavoriate");
			$(this).addClass("notInFavoriate");
			localStorage.removeItem($(this).data("key"));
			$removeTip.popup("open", { positionTo: $(this) });
			setTimeout(function () {
				$removeTip.popup("close");
			}, 500);
		}
	});
};

var onFavoriatePageInit = function () {
	$(document).on("swipeleft", ".favoriateList li.ui-li", function (event) {
		var listItem = $(this);
		var bookJson = localStorage.getItem(listItem.data("key"));
		var book = JSON.parse(bookJson);
		listItem.removeClass("done");
		book.IsDone = false;
		localStorage.setItem(listItem.data("key"), JSON.stringify(book));
	});
	$(document).on("swiperight", ".favoriateList li.ui-li", function (event) {
		var listItem = $(this);
		var bookJson = localStorage.getItem(listItem.data("key"));
		var book = JSON.parse(bookJson);
		if (!book.IsDone) {
			listItem.addClass("done");
			book.IsDone = true;
			localStorage.setItem(listItem.data("key"), JSON.stringify(book));
		}
		else {
			listItem.remove();
			localStorage.removeItem(listItem.data("key"));
		}
	});
};

var onFavoriatePageShow = function () {
	//我的收藏每次显示的时候加载localStorage项目
	var key, favoriateBook;
	var favoriateBooks = [];
	var doneFavoriateBooks = [];
	$(".favoriateList li").remove();
	for (i = 0; i < localStorage.length; i++) {
		key = localStorage.key(i);
		if (key[0] === "@") {
			favoriateBook = JSON.parse(localStorage.getItem(key));
			if (favoriateBook.IsDone) {
				doneFavoriateBooks.push(favoriateBook);
			} else {
				favoriateBooks.push(favoriateBook);
			}
		}
	}
	var $favoriateList = $(".favoriateList");
	var favoriateBooksTemplate = getFavoriateBooksTemplate();
	if (favoriateBooks.length > 0) {
		$(favoriateBooksTemplate(favoriateBooks)).appendTo($favoriateList);
	}
	if (doneFavoriateBooks.length > 0) {
		$(favoriateBooksTemplate(doneFavoriateBooks)).appendTo($favoriateList);
	}
	$favoriateList.listview("refresh");
	$("a[data-role='button']", $favoriateList).button();
};

var getBooksTemplate = function () {
	return Handlebars.compile($("#books").html());
};

var getFavoriateBooksTemplate = function () {
	return Handlebars.compile($("#favoriateBooks").html());
};

var librarySearch = function (keyword, page, onSearchDone, onSearchError, onAjaxError) {
	//图书馆查询页数最大值
	var MAX_PAGESIZE = 50;
	$.ajax("/Library/Search", { data: { keyword: keyword, page: page, pageSize: MAX_PAGESIZE } }).done(function (data, textStatus, jqXHR) {
		if (data.Error === null) {
			onSearchDone(data.Books, data.PageCount, data.ResultCount);
		} else {
			onSearchError(data.Error);
		}
	}).error(function (jqXHR, textStatus, errorThrown) {
		onAjaxError(textStatus, errorThrown);
	});
};

//获取url参数
//从网上抄的，不明觉厉
var getURLParameter = function (name) {
	return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null;
};

$(document).on("pageinit", "#favoriate", onFavoriatePageInit);
$(document).on("pageshow", "#favoriate", onFavoriatePageShow);
$(document).on("pageinit", "#search", onSearchPageInit);