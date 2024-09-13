<template>
  <v-stage
    ref="stage"
    :config="configKonva"
    @mousedown="handleMouseDown"
    @mousemove="handleMouseMove"
    @mouseup="handleMouseUp"
  >
    <v-layer ref="layer">
      <v-line
        v-for="line in lines"
        :key="line.id"
        :config="{
          stroke: 'white',
          points: line.points,
        }"
      />
    </v-layer>
  </v-stage>
</template>

<script setup lang="ts">
import { HttpTransportType, HubConnectionBuilder } from '@microsoft/signalr';
import { reactive, ref } from 'vue';
import { v4 as uuidv4 } from 'uuid';

const width = window.innerWidth;
const height = window.innerHeight;
const lines = reactive<ILine[]>([]);
const isDrawing = ref(false);
const configKonva = {
  width: width,
  height: height,
}

interface IPoint {
  x: number,
  y: number,
}

interface ILine {
  id: string,
  points: IPoint[],
}

function handleMouseDown(e: any) {
  isDrawing.value = true;
  const pos = e.target.getStage().getPointerPosition();

  const newLine: ILine = {
    id: uuidv4(),
    points: [pos.x, pos.y],
  }

  lines.push(newLine);
}

function handleMouseMove(e: any) {
  // no drawing - skipping
  if (!isDrawing.value) {
    return;
  }

  const stage = e.target.getStage();
  const point = stage.getPointerPosition();
  let lastLine = lines[lines.length - 1];
  // add point
  lastLine.points = lastLine.points.concat([point.x, point.y]);

  // replace last
  lines.splice(lines.length - 1, 1, lastLine);

  connection.invoke("LineUpdated", lastLine).catch(function (err) {
      return console.error(err.toString());
  });
}

function handleMouseUp() {
  isDrawing.value = false;
}

const connection = new HubConnectionBuilder()
  .withUrl(import.meta.env.VITE_HUB_URL, {
    skipNegotiation: true,
    transport: HttpTransportType.WebSockets,
  })
  .build();

connection.on("OnLineUpdated", (newLine: ILine) => {
  const line = lines.find(i => i.id == newLine.id);
  
  // Replace line
  if (line) {
    lines.splice(lines.indexOf(line), 1, newLine);
  } else {
    lines.unshift(newLine);
  }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});;

</script>
