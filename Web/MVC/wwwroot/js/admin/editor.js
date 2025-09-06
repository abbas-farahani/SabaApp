function CallCKEditor(element) {
    ClassicEditor.create(document.querySelector(element), {
        toolbar: [
            'undo',
            'redo',
            '|',
            'heading',
            '|',
            'bold',
            'italic',
            'underline',
            '|',
            'link',
            'uploadImage',
            'ckbox',
            'insertTable',
            'blockQuote',
            'mediaEmbed',
            '|',
            'bulletedList',
            'numberedList',
            '|',
            'outdent',
            'indent'
        ],
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
                }
            ]
        },
        image: {
            resizeOptions: [
                {
                    name: 'resizeImage:original',
                    label: 'Default image width',
                    value: null
                },
                {
                    name: 'resizeImage:50',
                    label: '50% page width',
                    value: '50'
                },
                {
                    name: 'resizeImage:75',
                    label: '75% page width',
                    value: '75'
                }
            ],
            toolbar: [
                'imageTextAlternative',
                'toggleImageCaption',
                '|',
                'imageStyle:inline',
                'imageStyle:wrapText',
                'imageStyle:breakText',
                '|',
                'resizeImage'
            ]
        },
        link: {
            addTargetToExternalLinks: true,
            defaultProtocol: 'https://'
        },
        table: {
            contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells']
        },
        language: 'ar'
    })
        .then(editor => {
            window.editor = editor;
        })
        .catch(error => {
            console.log(error);
        });
}
