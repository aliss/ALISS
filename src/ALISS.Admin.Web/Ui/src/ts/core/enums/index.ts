enum Elements {
	Body = 'body',
	Button = 'button',
	Head = 'head',
	Header = 'header',
	Footer = 'footer',
	Nav = 'nav',
	Section = 'section',
	Article = 'article',
	Image = 'img',
	Italic = 'i',
	Input = 'input',
	Emphasis = 'em',
	Strong = 'strong',
	Mark = 'mark',
	Cite = 'cite',
	Define = 'dfn',
	Aside = 'aside',
	Span = 'span',
	Script = 'script',
	Div = 'div',
	Html = 'html',
	Select = 'select',
	Option = 'option',
	Table = 'table',
	Textarea = 'textarea',
	Fieldset = 'fieldset',
	Unordered = "ul",
	Ordered = "ol",
	ListItem = "li",
	Video = 'video',
	Source = 'source',
	Paragraph = 'p',
	Iframe = 'iframe',
	Hyperlink = 'a'
}

enum InputTypes {
	Email = 'email',
	Number = 'number',
	Text = 'text',
	Radio = 'radio',
	Checkbox = 'checkbox'
}

enum Messages {
	BackToTop = 'Back to top'
}

enum Errors {
	Email = " contains an invalid email address.",
	Required = " is a required field",
	Phone = " contains an invalid phone number",
	Options = " has not been selected, please select one the following options.",
	Check = " has not been selected, please select at least one of options.",
}

enum Events {
	Beforeend = "beforeend",
	Bind = "bind",
	Blur = "blur",
	Click = "click",
	Change = "change",
	Focus = "focus",
	Keydown = "keydown",
	Keyup = "keyup",
	Load = "load",
	Mousedown = "mousedown",
	Mousemove = "mousemove",
	Mouseover = "mouseover",
	Mouseup = "mouseup",
	Resize = "resize",
	Scroll = "scroll",
	Submit = "submit",
	Touchstart = 'touchstart',
	Touchmove = 'touchmove',
	Touchend = 'touchend',
	Unbind = "unbind"
}

enum Positions {
	Top = "top",
	Left = "left",
	Bottom = "bottom",
	Right = "right",
	Up = "up",
	Down = "down"
}

enum Colors {
	Primary = "primary",
	Secondary = "secondary",
	Tertiary = "tertiary",
	Success = "success",
	Warning = "warning",
	Error = "error",
	Black = "black",
	DarkGray = "dark-gray",
	Gray = "gray",
	LightGray = "light-gray",
	White = "white"
}

export default {
	Elements,
	Errors,
	Events,
	InputTypes,
	Messages,
	Positions,
	Colors
}