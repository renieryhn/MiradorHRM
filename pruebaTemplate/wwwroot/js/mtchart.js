class MTChart {
    constructor(MTChart, setup) {
        this.MTChart = MTChart;
        this.setup = setup;

        const MTChartElement = document.getElementById(MTChart.id);
        Object.assign(MTChartElement.style, {
            height: setup.config.height,
            width: setup.config.width,
            overflow: "scroll",
            position: "relative",
        });

        this.setup.config.height = MTChartElement.clientHeight;
        this.setup.config.width = MTChartElement.clientWidth;
        console.log(this.setup.config.height, this.setup.config.width);
        this.update();
    }

    updateNodeList() {
        const { nodes } = this.setup;

        // Node list
        this.nodeList = nodes.map(() => []);

        nodes.forEach((node) => {
            if (node.pid != null) this.nodeList[node.pid].push(node.id);
        });

        // Node level
        this.nodeLevel = nodes.map(() => 0);

        nodes.forEach((node) => {
            const { id, pid } = node;
            this.nodeLevel[id] = pid != null ? this.nodeLevel[pid] + 1 : 0;
        });
    }

    update() {
        this.sortNodes();
        this.updateNodeList();

        const { nodes, config } = this.setup;
        const MTChart = this.MTChart;
        MTChart.innerHTML = "";

        const { widthNode: widthN, heightNode: heightN, margin } = config;
        const nodeLevel = this.nodeLevel;

        const nodeX = new Array(nodes.length).fill(0);
        const parentCurrWidth = new Array(nodes.length).fill(0);

        nodes.forEach((node) => {
            const { id, pid } = node;
            const nodeIdHTML = `${MTChart.id}${id}`;
            const y = nodeLevel[id] * (heightN + margin * 2) + heightN / 2;
            let x = 0;

            if (pid == null) {
                x = this.getRootX();
                nodeX[id] = x;
            } else {
                const parentWidth = this.maximumWidth(pid);
                const currWidth = this.maximumWidth(id);
                x = nodeX[pid] - parentWidth / 2 + parentCurrWidth[pid] + currWidth / 2;
                nodeX[id] = x;
                parentCurrWidth[pid] += currWidth;
            }

            const newNodeHTML = `
                <div id="${nodeIdHTML}" class="nodeBlock" style="position: absolute; top: ${y - heightN / 2}px; left: ${x - widthN / 2}px; width: ${widthN}px; height: ${heightN}px;" 
                ${config.canMove ? `draggable="true" ondrop="drop(event)" ondragover="allowDrop(event)" ondragstart="dragStart(event)">` : '>'}
                ${config.canMove ? `<i class="MTChartNodeMove fa fa-arrows-alt"></i>` : ''}
                ${config.canEdit ? `<i class="MTChartNodeEdit fa fa-pencil" onclick="editNode('${nodeIdHTML}')"></i>` : ''}
                ${config.canCreate ? `<i class="MTChartNodeCreate fa fa-plus" onclick="createNode('${nodeIdHTML}')"></i>` : ''}
                <span class="MTChartNodeText">${node.name}</span>
                </div>`;

            MTChart.innerHTML += newNodeHTML;
        });

        this.drawLines();
    }

    drawLines() {
        const { nodes } = this.setup;
        const MTChart = this.MTChart;
        const lineStyle = "2px solid black";
        const lineRadius = 20;

        let minX = 0;
        let minY = 0;

        nodes.forEach((node) => {
            const nodeIdHTML = `${MTChart.id}${node.id}`;
            const { top, left } = document.getElementById(nodeIdHTML).getBoundingClientRect();

            minX = Math.min(minX, left);
            minY = Math.min(minY, top);
        });

        nodes.forEach((node) => {
            const nodeIdHTML = `${MTChart.id}${node.id}`;
            const { top: y, left: x } = document.getElementById(nodeIdHTML).getBoundingClientRect();
            const nodeCenterX = x + document.getElementById(nodeIdHTML).offsetWidth / 2;
            const nodeCenterY = y + document.getElementById(nodeIdHTML).offsetHeight / 2;

            this.nodeList[node.id].forEach((cid) => {
                const childNodeIdHTML = `${MTChart.id}${cid}`;
                const { top: cy, left: cx } = document.getElementById(childNodeIdHTML).getBoundingClientRect();
                const childCenterX = cx + document.getElementById(childNodeIdHTML).offsetWidth / 2;
                const childCenterY = cy + document.getElementById(childNodeIdHTML).offsetHeight / 2;

                const lineHTML = `
                    <div style="position: absolute; 
                    left: ${Math.min(nodeCenterX, childCenterX) - minX}px;
                    top: ${nodeCenterY - minY}px;
                    height: ${childCenterY - nodeCenterY}px;
                    width: ${Math.abs(childCenterX - nodeCenterX)}px;
                    border-${childCenterX > nodeCenterX ? 'right' : 'left'}: ${lineStyle};
                    border-top: ${lineStyle};
                    border-radius: ${lineRadius}px;
                    z-index: -1;"></div>`;

                MTChart.innerHTML = lineHTML + MTChart.innerHTML;
            });
        });
    }

    maximumWidth(nodeId) {
        const { widthNode: widthN, margin } = this.setup.config;

        return this.nodeList[nodeId].length === 0
            ? widthN + margin * 2
            : this.nodeList[nodeId].reduce((acc, cid) => acc + this.maximumWidth(cid), 0);
    }

    getRootX() {
        const { nodes, config } = this.setup;

        for (const node of nodes) {
            if (node.pid == null) {
                return this.maximumWidth(node.id) / 2;
            }
        }

        return config.width / 2;
    }

    getNodeById(id) {
        return this.setup.nodes.find(node => node.id === id);
    }

    sortNodes() {
        this.setup.nodes.sort((a, b) => (a.level || 0) - (b.level || 0));
    }

    isChild(parent, child) {
        return this.nodeList[parent].some(id => id === child || this.isChild(id, child));
    }

    lastId() {
        return Math.max(...this.setup.nodes.map(node => node.id));
    }
}

// Helper functions
function dragStart(event) {
    event.dataTransfer.setData("Text", event.target.id);
}

function allowDrop(event) {
    event.preventDefault();
}

function drop(event) {
    event.preventDefault();
    const dragName = event.dataTransfer.getData("Text");
    const dropName = event.target.id;

    const dragId = dragName.split(chart.MTChart.id)[1];
    const dropId = dropName.split(chart.MTChart.id)[1];

    if (dragId && dropId && dragId !== dropId && !chart.isChild(dragId, dropId)) {
        chart.getNodeById(dragId).pid = parseInt(dropId);
        chart.update();
    }
}

function createNode(parentIdHTML) {
    const parentId = parentIdHTML.split(chart.MTChart.id)[1];
    chart.setup.nodes.push({ id: chart.lastId() + 1, pid: parseInt(parentId), name: "New node" });
    chart.update();
}

function editNode(nodeIdHTML) {
    // Implement edit logic
}
