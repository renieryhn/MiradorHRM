namespace MiradorHRM.wwwroot
{
    public class empleado_tabs
    {     
        constructor(idEmpleado) {
            this.idEmpleado = idEmpleado;
            this.registerTabNavigation();
            this.restoreActiveTab();
        }

        getFilter(tab) {
            return $(`#filter${tab}`).val() || "";
        }

        loadTabData(tabId, spinnerId = '') {
            let tab = tabId;
            let funcion = '';
            let targetDiv = '';

            if (tab === 'custom-tabs-two-profile-tab') {
                funcion = 'LoadIngreso';
                targetDiv = '#custom-tabs-two-profile';
            } else if (tab === 'custom-tabs-two-messages-tab') {
                funcion = 'LoadImpuesto';
                targetDiv = '#custom-tabs-two-messages';
            } else if (tab === 'custom-tabs-two-settings-tab') {
                funcion = 'LoadDeduccion';
                targetDiv = '#custom-tabs-two-settings';
            }

            if (spinnerId) {
                $('#' + spinnerId).show();
            }

            const surl = `/NominaEmpleado/${funcion}?id=${this.idEmpleado}&filter=`;

            $.ajax({
                url: surl,
                method: 'GET',
                success: (data) => {
                    $(targetDiv).html(data);
                    this.initializeSearchEvents(tab); // reactivar búsqueda
                    if (spinnerId) $('#' + spinnerId).hide();
                },
                error: (xhr, status, error) => {
                    console.error(error);
                    if (spinnerId) $('#' + spinnerId).hide();
                }
            });
        }

        buscar(tab, divTarget) {
            const filter = this.getFilter(tab);
            const funcion = this.getFunctionName(tab);
            const url = `/NominaEmpleado/${funcion}?id=${this.idEmpleado}&filter=${encodeURIComponent(filter)}`;

            $.get(url, (data) => {
                $(divTarget).html(data);
                this.initializeSearchEvents(tab);
            }).fail(() => {
                toastr.error('Error al filtrar.');
            });
        }

        paginar(tab, pagina) {
            const filter = this.getFilter(tab);
            const funcion = this.getFunctionName(tab);
            const divTarget = this.getTargetDiv(tab);

            const url = `/NominaEmpleado/${funcion}?id=${this.idEmpleado}&filter=${encodeURIComponent(filter)}&pg=${pagina}`;

            $.get(url, (data) => {
                $(divTarget).html(data);
                this.initializeSearchEvents(tab);
            });
        }

        getFunctionName(tab) {
            switch (tab) {
                case 'Ingreso': return 'LoadIngreso';
                case 'Impuesto': return 'LoadImpuesto';
                case 'Deduccion': return 'LoadDeduccion';
            }
        }

        getTargetDiv(tab) {
            switch (tab) {
                case 'Ingreso': return '#custom-tabs-two-profile';
                case 'Impuesto': return '#custom-tabs-two-messages';
                case 'Deduccion': return '#custom-tabs-two-settings';
            }
        }

        initializeSearchEvents(tab) {
            const inputId = `#filter${tab}`;
            $(inputId).off('keypress').on('keypress', (e) => {
                if (e.which === 13) this.buscar(tab, this.getTargetDiv(tab));
            });
        }

        registerTabNavigation() {
            $('.nav-link').on('click', function () {
                const tabId = $(this).attr('id');
                sessionStorage.setItem('activeTab', tabId);
            });
        }

        restoreActiveTab() {
            const activeTab = sessionStorage.getItem('activeTab');
            if (activeTab) {
                $('#' + activeTab).tab('show');
                const tabKey = activeTab.replace('custom-tabs-two-', '').replace('-tab', '');
                const spinnerId = 'loading-spinner-' + tabKey;
                this.loadTabData(activeTab, spinnerId);
            }
        }
    

    }
}
