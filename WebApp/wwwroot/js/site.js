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

        document.addEventListener('click', async (e) => {
            const btn = e.target.closest('.btn-edit');
            if (!btn) return;

            const projectId = btn.closest('.card.project').dataset.projectId;
            if (!projectId) return;

            const resp = await fetch(`/admin/projects/${projectId}`);
            if (!resp.ok) {
                console.error('Project data not found', resp.status);
                return;
            }
            const data = await resp.json();

            const modal = document.querySelector('#edit-project-modal');
            modal.querySelector('input[name="Id"]').value = data.id;
            modal.querySelector('input[name="ProjectName"]').value = data.projectName;
            modal.querySelector('textarea[name="Description"]').value = data.description || '';
            modal.querySelector('input[name="StartDate"]').value = data.startDate?.split('T')[0] || '';
            modal.querySelector('input[name="EndDate"]').value = data.endDate?.split('T')[0] || '';
            modal.querySelector('input[name="Budget"]').value = data.budget || '';

            const buildOptions = (items, selectedValue) =>
                items.map(i =>
                    `<option value="${i.value}" ${i.value === selectedValue ? 'selected' : ''}>${i.text}</option>`
                ).join('');

            modal.querySelector('select[name="ClientId"]').innerHTML =
                `<option value="">Choose a Client</option>` +
                buildOptions(data.clients, data.clientId);

            modal.querySelector('select[name="MemberId"]').innerHTML =
                `<option value="">Choose a Member</option>` +
                buildOptions(data.members, data.memberId);

            modal.querySelector('select[name="StatusId"]').innerHTML =
                `<option value="">Choose a Status</option>` +
                buildOptions(data.statuses, data.statusId);

            modal.classList.add('modal-show');
        });

        document.addEventListener('click', async e => {
            const btn = e.target.closest('[data-type="delete"]');
            if (!btn) return;

            if (!confirm('Are you sure you want to delete this project?'))
                return;

            const projectId = btn.dataset.projectId;
            try {
                const resp = await fetch(`/admin/projects/delete/${projectId}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document
                            .querySelector('input[name="__RequestVerificationToken"]')
                            .value
                    }
                });
                if (!resp.ok) throw new Error(`Status ${resp.status}`);

                btn.closest('.card.project').remove();
            } catch (err) {
                console.error('Project was not deleted', err);
                alert('Project was not deleted');
            }
        });
    }

    function clearErrorMessages(form) {
        form.querySelectorAll('[data-val="true"]').forEach(input => {
            input.classList.remove('input.validation-error')
        })

        form.querySelectorAll('[data-valmsg-for]').forEach(span => {
            span.innerText = ''
            span.classList.remove('field-validation-error')
        })
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


