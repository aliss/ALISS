import Aliss from '../index';

function isInternetExplorer() {
	const ua = window.navigator.userAgent;
	const msie = ua.indexOf('MSIE ');
	if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
		// If Internet Explorer, return version number
		document.createElement(Aliss.Enums.Elements.Header);
		document.createElement(Aliss.Enums.Elements.Nav);
		document.createElement(Aliss.Enums.Elements.Article);
		document.createElement(Aliss.Enums.Elements.Section);
		document.createElement(Aliss.Enums.Elements.Aside);
		document.createElement(Aliss.Enums.Elements.Footer);
		document.body.classList.add("ie11");
	}
	return false;
}

function isEmpty(value: any) {
	return value === null || value === undefined;
}

function isVariableEmpty(variable: any) {
	return variable !== null && variable !== undefined && variable !== '';
}

function isUrl(string: any) {
	const regexp = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/;
	return regexp.test(string);
}

function isFunction(func: any) {
	const getType = {};
	return func && getType.toString.call(func) === '[object Function]';
}

function getElementByClass(el: any) {
	return document.querySelector('.' + el);
}

function getTag(el: any) {
	return document.querySelector(el);
}

function getClass(string: any) {
	return '.' + string;
}

function getId(string: any) {
	return '#' + string;
}

function getAttribute(string: any) {
	return '[' + string + ']';
}

function getFullAttribute(attribute: any, value: any) {
	return '[' + attribute + '="' + value + '"]';
}

function forEach(selector: any, fn: any) {
	const elements = document.querySelectorAll(selector);
	for (var i = 0; i < elements.length; i++)
		fn(elements[i], i);
}

function forEachInner(el: Element, selector: any, fn: any) {
	const elements = el.querySelectorAll(selector);
	for (let i = 0; i < elements.length; i++)
		fn(elements[i], i);
}

function toggleClass(el: any, className: string) {
	if (el.classList) {
		el.classList.toggle(className);
	} else {
		let classes = el.className.split(' ');
		let existingIndex = -1;
		for (let i = classes.length; i--;) {
			if (classes[i] === className) existingIndex = i;
		}
		if (existingIndex >= 0) classes.splice(existingIndex, 1);
		else classes.push(className);
		el.className = classes.join(' ');
	}
}

function hasClass(el: any, className: string) {
	if (el.classList) return el.classList.contains(className);
	else
		return new RegExp('(^| )' + className + '( |$)', 'gi').test(el.className);
}

function addClass(el: any, className: any) {
	if (el.classList) el.classList.add(className);
	else el.className += ' ' + className;
}

function removeClass(el: any, className: any) {
	if (el.classList) el.classList.remove(className);
	else
		el.className = el.className.replace(
			new RegExp(
				'(^|\\b)' +
				className.split(' ').join('|') +
				'(\\b|document.querySelector)',
				'gi'
			),
			' '
		);
}

function nextElement(el: any) {
	function nextElementSibling(el: any) {
		do {
			el = el.nextSibling;
		} while (el && el.nodeType !== 1);
		return el;
	}
	return el.nextElementSibling || nextElementSibling(el);
}

function speak(message: string) {
	let msg = new SpeechSynthesisUtterance(message);
	let voices = window.speechSynthesis.getVoices();
	msg.voice = voices[0];
	window.speechSynthesis.speak(msg);
}

function closest(el: any, selector: any) {
	let matchesSelector =
		el.matches ||
		el.webkitMatchesSelector ||
		el.mozMatchesSelector ||
		el.msMatchesSelector;
	while (el) {
		if (matchesSelector.call(el, selector)) {
			break;
		}
		el = el.parentElement;
	}
	return el;
}

function removeElementById(elementId: string) {
	const element = document.getElementById(elementId) as HTMLElement;
	if (element.parentNode !== null)
		return element.parentNode.removeChild(element);
	else return new Error('Sorry no parent element can be detected.');
}

function removeElementByNode(node: Node) {
	if (node.parentNode !== null)
		return node.parentNode.removeChild(node);
	else return new Error('Sorry no parent element can be detected.');
}

function getJson(url: string, callback: Function) {
	let xhr = new XMLHttpRequest();
	xhr.open('GET', url, true);
	xhr.onload = () => {
		if (xhr.status >= 200 && xhr.status < 400) {
			callback(xhr.responseText);
		}
	};
	xhr.send();
}

function submitForm(url: String, data: any, success: any, error: any) {
	let xmlhttp: any;
	xmlhttp = new XMLHttpRequest();
	xmlhttp.onreadystatechange = () => {
		if (xmlhttp.readyState === 4) {
			if (xmlhttp.status === 200) {
				success(xmlhttp);
			} else {
				if (error) {
					console.log(error);
					var msg = JSON.parse(xmlhttp.response);
					error(msg);
				}
			}
		}
	};

	xmlhttp.open("POST", url, true);
	xmlhttp.setRequestHeader("Content-type", "application/json;charset=UTF-8");
	xmlhttp.send(JSON.stringify(data));
}

function getAjax(url: string, callback: Function) {
	let xhr = new XMLHttpRequest();
	xhr.open('GET', url);
	xhr.onload = function () {
		if (xhr.status >= 200 && xhr.status < 400) {
			callback(xhr.responseText);
		}
	};
	xhr.send();
}

function sendAjax(url: string, data: any) {
	var request = new XMLHttpRequest();
	request.open('POST', url, true);
	request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
	request.send(data);
}

function loadExternalJS(url: string){
	var script = document.createElement("script") as HTMLScriptElement;  // create a script DOM node
	script.src = url;  // set its src to the provided URL
	document.head.appendChild(script);
}

function simulateClick(elem: HTMLElement) {
	// Create our event (with options)
	var evt = new MouseEvent('click', {
		bubbles: true,
		cancelable: true,
		view: window
	});
	// If cancelled, don't dispatch our event
	var canceled = !elem.dispatchEvent(evt);
}

function sendJson(url: string, body: any, callback: Function) {
	let xhr = new XMLHttpRequest();
	xhr.open('POST', url);
	xhr.setRequestHeader('Content-Type', 'application/json');
	xhr.onload = function () {
		if (xhr.status >= 200 && xhr.status < 400) {
			callback(xhr.responseText);
		}
	};
	xhr.send(JSON.stringify(body));
}

function trigger(element: Element, event: string) {
	let changeEvent = new Event(event);
	element.dispatchEvent(changeEvent);
}

function setCookie(cookieName: string, cookieValue: any, daysToExpire: any) {
	let date = new Date();
	date.setTime(date.getTime() + daysToExpire * 24 * 60 * 60 * 1000);
	document.cookie =
		cookieName + '=' + cookieValue + '; expires=' + date.toUTCString();
}

function getCookie(cookieName: string) {
	var cookieValue = document.cookie.match(
		'(^|;)\\s*' + cookieName + '\\s*=\\s*([^;]+)'
	);
	return cookieValue ? cookieValue.pop() : '';
}

function getFullCookie(name: String){
	return document.cookie.split(';')
		.map(function(cookie){
			var splitCookie = cookie.split("=");
			return {
				name: splitCookie[0].trim(),
				value: splitCookie[1]
			}
		})
		.filter(function(cookie){
			return cookie.name === name;
		})[0];
}

function eatCookies() {
	console.log('checking cookies');
	var cookies = document.cookie.split("; ");
	for (var c = 0; c < cookies.length; c++) {
		var d = window.location.hostname.split(".");
		if(cookies[c].split(";")[0].split("=")[0] !== 'cookieControl') {
			while (d.length > 0) {
				var cookieBase = encodeURIComponent(cookies[c].split(";")[0].split("=")[0]) + '=; expires=Thu, 01-Jan-1970 00:00:01 GMT; domain=' + d.join('.') + ' ;path=';
				var p = location.pathname.split('/');
				Aliss.Helpers.deleteCookie(cookieBase + '/');
				console.log('COOKIE BASE ' + cookieBase);

				d.shift();
			}
		}
	}
	for (var cc = 0; cc < cookies.length; cc++) {
		var dd = window.location.hostname.split(".");
		if(cookies[c].split(";")[0].split("=")[0] !== 'cookieControl') {
			while (dd.length > 0) {
				var defaultBase = encodeURIComponent(cookies[c].split(";")[0].split("=")[0]) + '=; expires=Thu, 01-Jan-1970 00:00:01 GMT;path=';
				var pp = location.pathname.split('/');
				Aliss.Helpers.deleteCookie(defaultBase + '/');
				console.log('DEFAULT BASE ' + defaultBase);
				dd.shift();
			}
		}
	}
}

function deleteCookie(cookieName: string) {
	document.cookie = cookieName + "=''; max-age=0; expires=0";
}

function closeCookie(cookieName: String, root: HTMLElement, child: HTMLDivElement){
	var cookieExpire = new Date();
	cookieExpire.setFullYear(cookieExpire.getFullYear() +2);
	document.cookie = cookieName + "=1; expires=" + cookieExpire.toUTCString() + ";";
	root.removeChild(child);
}

function extendObject(def: any, addons: any) {
	if (typeof addons !== 'undefined') {
		for (let prop in def) {
			if (addons[prop] != undefined) {
				def[prop] = addons[prop];
			}
		}
	}
}

function checkVersionIe(range: any) {
	let query = new RegExp(`msie\\s[${range}]`, "i");
	return !!navigator.userAgent.match(query);
}

let lazy = [] as any;

function setLazy() {
	lazy = document.querySelectorAll('[data-src]');
}

function lazyLoad() {
	for (var i = 0; i < lazy.length; i++) {
		if (isInViewport(lazy[i])) {
			if (lazy[i].getAttribute('data-src')) {
				lazy[i].src = lazy[i].getAttribute('data-src');
				lazy[i].removeAttribute('data-src');
			}
		}
	}
	cleanLazy();
}

function cleanLazy() {
	lazy = Array.prototype.filter.call(lazy, (l: Element) => { return l.getAttribute('data-src'); });
}

function isInViewport(el: Element) {
	let rect = el.getBoundingClientRect();
	return (
		rect.bottom >= 0 &&
		rect.right >= 0 &&
		rect.top <= (window.innerHeight || document.documentElement.clientHeight) &&
		rect.left <= (window.innerWidth || document.documentElement.clientWidth)
	);
}

function getNodeId(document: any) {
	try {
		return document.querySelector("meta[name='umb:id']").getAttribute('content');
	} catch (e) {
		return '';
	}
}

function getDocType(document: any) {
	try {
		return document.querySelector("meta[name='umb:docType']").getAttribute('content');
	} catch (e) {
		return '';
	}
}

function nthParent(element: any, n: any) {
	while (n-- && element)
		element = element.parentNode;
	return element;
}

let regExpressions = {
	phone: /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/,
	email: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
}

export default {
	addClass,
	closest,
	checkVersionIe,
	cleanLazy,
	closeCookie,
	deleteCookie,
	extendObject,
	isEmpty,
	isFunction,
	isInternetExplorer,
	isInViewport,
	isUrl,
	isVariableEmpty,
	eatCookies,
	forEach,
	forEachInner,
	getAjax,
	getAttribute,
	getClass,
	getCookie,
	getFullCookie,
	getDocType,
	getElementByClass,
	getFullAttribute,
	getId,
	getJson,
	getNodeId,
	getTag,
	hasClass,
	lazyLoad,
	loadExternalJS,
	nextElement,
	nthParent,
	regExpressions,
	removeClass,
	removeElementById,
	removeElementByNode,
	setCookie,
	setLazy,
	sendAjax,
	sendJson,
	simulateClick,
	speak,
	submitForm,
	toggleClass,
	trigger
};
