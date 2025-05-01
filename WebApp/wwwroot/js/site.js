(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', () => {
        initQuillEditors();
        initFilePreviews();
        initDropdowns();
        initModals();
        initCustomSelects();
    });

    function initQuillEditors() {
        document.querySelectorAll('.wysiwyg').forEach(container => {
            const mode = container.dataset.wysiwyg; 
            const textarea = container.querySelector('textarea');
            const editorEl = container.querySelector('.wysiwyg-editor');
            const toolbarEl = container.querySelector('.wysiwyg-toolbar');

            const quill = new Quill(editorEl, {
                modules: {
                    syntax: true,
                    toolbar: toolbarEl
                },
                theme: 'snow',
                placeholder: 'Type something'
            });

            if (textarea.value) quill.root.innerHTML = textarea.value;

            quill.on('text-change', () => {
                textarea.value = quill.root.innerHTML;
            });
        });
    }


    function initFilePreviews() {
        document.querySelectorAll('[data-modal]').forEach(modal => {
            const trigger = modal.querySelector('[data-upload-trigger]');
            const input = modal.querySelector('input[type="file"]');
            const preview = modal.querySelector('.image-preview');
            const iconCt = modal.querySelector('.image-preview-icon-container');
            const icon = modal.querySelector('.image-preview-icon');

            if (!trigger || !input) return;

            trigger.addEventListener('click', () => input.click());

            input.addEventListener('change', e => {
                const file = e.target.files[0];
                if (file && file.type.startsWith('image/')) {
                    const reader = new FileReader();
                    reader.onload = evt => {
                        preview.src = evt.target.result;
                        preview.classList.remove('hide');
                        iconCt.classList.add('selected');
                        icon.classList.replace('fa-camera', 'fa-pen-to-square');
                    };
                    reader.readAsDataURL(file);
                }
            });
        });
    }


    function initDropdowns() {
        const dropdownBtns = document.querySelectorAll('[data-type="dropdown"]');
        document.addEventListener('click', e => {
            let opened = false;
            dropdownBtns.forEach(btn => {
                const target = document.querySelector(btn.dataset.target);
                if (btn.contains(e.target)) {
                    opened = true;
                    
                    document.querySelectorAll('.dropdown-show').forEach(d => d !== target && d.classList.remove('dropdown-show'));
                    target.classList.toggle('dropdown-show');
                }
            });
            if (!opened && !e.target.closest('.dropdown')) {
                document.querySelectorAll('.dropdown-show').forEach(d => d.classList.remove('dropdown-show'));
            }
        });
    }

    function initModals() {

        document.querySelectorAll('[data-type="modal"]').forEach(btn => {
            btn.addEventListener('click', () => {
                const modal = document.querySelector(btn.dataset.target);
                modal?.classList.add('modal-show');
            });
        });

        document.querySelectorAll('[data-modal-close]').forEach(btn => {
            btn.addEventListener('click', () => {

                const modal = btn.closest('.modal');
                modal?.classList.remove('modal-show');
            });
        });
    }


    function initCustomSelects() {
        document.querySelectorAll('.forms-select').forEach(select => {
            const trigger = select.querySelector('.form-select-trigger');
            const options = select.querySelectorAll('.form-select-option');
            const hidden = select.querySelector('input[type="hidden"]');
            const placeholder = select.dataset.placeholder || 'Choose';

            const setValue = (val = '', txt = placeholder) => {
                trigger.querySelector('.form-select-text').textContent = txt;
                hidden.value = val;
                select.classList.toggle('has-placeholder', !val);
            };
            setValue();

            trigger.addEventListener('click', e => {
                e.stopPropagation();
                document.querySelectorAll('.forms-select.open').forEach(s => s !== select && s.classList.remove('open'));
                select.classList.toggle('open');
            });
            options.forEach(opt => {
                opt.addEventListener('click', () => {
                    setValue(opt.dataset.value, opt.textContent);
                    select.classList.remove('open');
                });
            });
            document.addEventListener('click', e => {
                if (!select.contains(e.target)) select.classList.remove('open');
            });
        });
    }

})();

