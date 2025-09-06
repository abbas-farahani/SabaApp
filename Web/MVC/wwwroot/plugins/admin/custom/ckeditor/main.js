/**
 * This configuration was generated using the CKEditor 5 Builder. You can modify it anytime using this link:
 * https://ckeditor.com/ckeditor-5/builder/#installation/NoNgNARATAdA7DADBSBGOAWKI4A5eKpS4DMGArLkViahiBrgJz13lPYi4jrlQoQApgDsUiMMFRgpM6WEQBdSCT4kAxgDNkCoA===
 */

import { ClassicEditor, Alignment, Autoformat, AutoImage, Autosave, BalloonToolbar, BlockQuote, BlockToolbar, Bold, CodeBlock, Emoji, Essentials, FindAndReplace, Fullscreen, GeneralHtmlSupport, Heading, HorizontalLine, HtmlEmbed, ImageBlock, ImageCaption, ImageInline, ImageInsert, ImageInsertViaUrl, ImageResize, ImageStyle, ImageTextAlternative, ImageToolbar, ImageUpload, Indent, IndentBlock, Italic, Link, LinkImage, List, ListProperties, Markdown, Mention, Paragraph, PasteFromMarkdownExperimental, PasteFromOffice, ShowBlocks, SimpleUploadAdapter, SpecialCharacters, SpecialCharactersArrows, SpecialCharactersCurrency, SpecialCharactersEssentials, SpecialCharactersLatin, SpecialCharactersMathematical, SpecialCharactersText, Style, Table, TableCaption, TableCellProperties, TableColumnResize, TableProperties, TableToolbar, TextTransformation, TodoList, Underline } from 'ckeditor5';

/**
 * Create a free account with a trial: https://portal.ckeditor.com/checkout?plan=free
 */
const editorConfig = {
	toolbar: {
		items: ['undo', 'redo', '|', 'showBlocks', '|', 'heading', 'style', '|', 'bold', 'italic', 'underline', '|', 'link', 'insertImage', 'insertTable', 'blockQuote', 'codeBlock', '|', 'alignment', '|', 'bulletedList', 'numberedList', 'todoList', 'outdent', 'indent'],
		shouldNotGroupWhenFull: false
	},
	plugins: [Alignment, Autoformat, AutoImage, Autosave, BalloonToolbar, BlockQuote, BlockToolbar, Bold, CodeBlock, Emoji, Essentials, FindAndReplace, Fullscreen, GeneralHtmlSupport, Heading, HorizontalLine, HtmlEmbed, ImageBlock, ImageCaption, ImageInline, ImageInsert, ImageInsertViaUrl, ImageResize, ImageStyle, ImageTextAlternative, ImageToolbar, ImageUpload, Indent, IndentBlock, Italic, Link, LinkImage, List, ListProperties, Markdown, Mention, Paragraph, PasteFromMarkdownExperimental, PasteFromOffice, ShowBlocks, SimpleUploadAdapter, SpecialCharacters, SpecialCharactersArrows, SpecialCharactersCurrency, SpecialCharactersEssentials, SpecialCharactersLatin, SpecialCharactersMathematical, SpecialCharactersText, Style, Table, TableCaption, TableCellProperties, TableColumnResize, TableProperties, TableToolbar, TextTransformation, TodoList, Underline],
	balloonToolbar: ['bold', 'italic', '|', 'link', 'insertImage', '|', 'bulletedList', 'numberedList'],
	blockToolbar: ['bold', 'italic', '|', 'link', 'insertImage', 'insertTable', '|', 'bulletedList', 'numberedList', 'outdent', 'indent'],
	fullscreen: {
		onEnterCallback: container =>
			container.classList.add('editor-container', 'editor-container_classic-editor', 'editor-container_include-style', 'editor-container_include-block-toolbar', 'editor-container_include-fullscreen', 'main-container')
	},
	heading: {
		options: [
			{
				model: 'paragraph',
				title: 'Paragraph',
				class: 'ck-heading_paragraph'
			},
			{
				model: 'heading1',
				view: 'h1',
				title: 'Heading 1',
				class: 'ck-heading_heading1'
			},
			{
				model: 'heading2',
				view: 'h2',
				title: 'Heading 2',
				class: 'ck-heading_heading2'
			},
			{
				model: 'heading3',
				view: 'h3',
				title: 'Heading 3',
				class: 'ck-heading_heading3'
			},
			{
				model: 'heading4',
				view: 'h4',
				title: 'Heading 4',
				class: 'ck-heading_heading4'
			},
			{
				model: 'heading5',
				view: 'h5',
				title: 'Heading 5',
				class: 'ck-heading_heading5'
			},
			{
				model: 'heading6',
				view: 'h6',
				title: 'Heading 6',
				class: 'ck-heading_heading6'
			}
		]
	},
	htmlSupport: {
		allow: [
			{
				name: /^.*$/,
				styles: true,
				attributes: true,
				classes: true
			}
		]
	},
	image: {
		toolbar: [ 'toggleImageCaption', 'imageTextAlternative', '|', 'imageStyle:inline', 'imageStyle:wrapText', 'imageStyle:breakText', '|', 'resizeImage' ]
	},
	initialData: 'Initial Rendering Text',
	licenseKey: 'GPL',
	link: {
		addTargetToExternalLinks: true,
		defaultProtocol: 'https://',
		decorators: {
			toggleDownloadable: {
				mode: 'manual',
				label: 'Downloadable',
				attributes: {
					download: 'file'
				}
			}
		}
	},
	list: {
		properties: {
			styles: true,
			startIndex: true,
			reversed: true
		}
	},
	mention: {
		feeds: [
			{
				marker: '@',
				feed: [ /* See: https://ckeditor.com/docs/ckeditor5/latest/features/mentions.html */ ]
			}
		]
	},
	menuBar: {
		isVisible: true
	},
	placeholder: 'Type or paste your content here!',
	style: {
		definitions: [
			{
				name: 'Article category',
				element: 'h3',
				classes: ['category']
			},
			{
				name: 'Title',
				element: 'h2',
				classes: ['document-title']
			},
			{
				name: 'Subtitle',
				element: 'h3',
				classes: ['document-subtitle']
			},
			{
				name: 'Info box',
				element: 'p',
				classes: ['info-box']
			},
			{
				name: 'CTA Link Primary',
				element: 'a',
				classes: ['button', 'button--green']
			},
			{
				name: 'CTA Link Secondary',
				element: 'a',
				classes: ['button', 'button--black']
			},
			{
				name: 'Marker',
				element: 'span',
				classes: ['marker']
			},
			{
				name: 'Spoiler',
				element: 'span',
				classes: ['spoiler']
			}
		]
	},
	table: {
		contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells', 'tableProperties', 'tableCellProperties']
	}
};

ClassicEditor.create(document.querySelector('#editor'), editorConfig);
