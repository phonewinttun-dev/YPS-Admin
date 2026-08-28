// Leaflet OSM Interop for YPS Route Stops Management (DESIGN.md semantic tokens)
(function () {
    const mapInstances = {};

    window.YpsRouteMap = {
        initMap: function (containerId, dotNetHelper, initialLat = 16.8661, initialLon = 96.1951, zoom = 12) {
            const container = document.getElementById(containerId);
            if (!container) return false;

            // If already initialized, remove previous instance
            if (mapInstances[containerId]) {
                try {
                    mapInstances[containerId].map.remove();
                } catch (e) {
                    console.warn("Error cleaning previous map instance", e);
                }
                delete mapInstances[containerId];
            }

            // Create Leaflet Map
            const map = L.map(containerId, {
                zoomControl: false,
                attributionControl: true
            }).setView([initialLat, initialLon], zoom);

            // Add Zoom Control to top right for better UI layout
            L.control.zoom({ position: 'topright' }).addTo(map);

            // Add standard OpenStreetMap tiles
            L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright" target="_blank" rel="noopener">OpenStreetMap</a> contributors'
            }).addTo(map);

            // Create layer groups
            const routeMarkersLayer = L.layerGroup().addTo(map);
            const polylineLayer = L.layerGroup().addTo(map);
            const allStopsLayer = L.layerGroup().addTo(map);

            mapInstances[containerId] = {
                map: map,
                dotNetHelper: dotNetHelper,
                routeMarkersLayer: routeMarkersLayer,
                polylineLayer: polylineLayer,
                allStopsLayer: allStopsLayer,
                markerMap: {}
            };

            // Invalidate size on container resize
            setTimeout(() => {
                map.invalidateSize();
            }, 200);

            return true;
        },

        renderRoute: function (containerId, stops) {
            const instance = mapInstances[containerId];
            if (!instance) return;

            const { map, routeMarkersLayer, polylineLayer } = instance;
            routeMarkersLayer.clearLayers();
            polylineLayer.clearLayers();
            instance.markerMap = {};

            if (!stops || stops.length === 0) {
                return;
            }

            const latLngs = [];

            stops.forEach((stop, index) => {
                if (stop.lat && stop.lon) {
                    const pos = [stop.lat, stop.lon];
                    latLngs.push(pos);

                    const isFirst = index === 0;
                    const isLast = index === stops.length - 1;

                    // DESIGN.md semantic colors: GPS (#137455) for start, Store (#A23F2B) for terminus, Bus (#0B5F9B) for intermediate
                    let badgeBg = 'background-color: #0B5F9B; color: #FFFFFF; border-color: #FFFFFF;';
                    if (isFirst) {
                        badgeBg = 'background-color: #137455; color: #FFFFFF; border-color: #FFFFFF;'; // Mint / GPS
                    } else if (isLast) {
                        badgeBg = 'background-color: #A23F2B; color: #FFFFFF; border-color: #FFFFFF;'; // Coral / Terminus
                    }

                    const iconHtml = `
                        <div class="flex items-center justify-center w-7 h-7 rounded-full border-2 shadow-md font-mono font-bold text-xs transition-transform hover:scale-125 cursor-pointer" style="${badgeBg}" title="${stop.stopName || 'Stop ' + stop.stopOrder}">
                            ${stop.stopOrder}
                        </div>
                    `;

                    const customIcon = L.divIcon({
                        html: iconHtml,
                        className: 'yps-map-stop-marker',
                        iconSize: [28, 28],
                        iconAnchor: [14, 14],
                        popupAnchor: [0, -14]
                    });

                    const marker = L.marker(pos, { icon: customIcon });

                    const popupContent = `
                        <div class="p-1 text-xs font-sans">
                            <div class="font-bold text-sm text-[#121521]">#${stop.stopOrder} - ${stop.stopName}</div>
                            <div class="text-[#5A6275] mt-0.5">${stop.regionName || 'No Region'}</div>
                            <div class="font-mono text-2xs text-[#9BA4B8] mt-1">${stop.lat.toFixed(5)}, ${stop.lon.toFixed(5)}</div>
                        </div>
                    `;
                    marker.bindPopup(popupContent);

                    marker.on('click', () => {
                        if (instance.dotNetHelper) {
                            instance.dotNetHelper.invokeMethodAsync('OnMapStopClicked', stop.busStopId);
                        }
                    });

                    routeMarkersLayer.addLayer(marker);
                    instance.markerMap[stop.busStopId] = marker;
                }
            });

            // Draw polyline connecting stops using Route violet (#6546AD)
            if (latLngs.length > 1) {
                // Background outline for polyline
                const polylineBg = L.polyline(latLngs, {
                    color: '#ffffff',
                    weight: 6,
                    opacity: 0.9,
                    lineJoin: 'round'
                });
                polylineLayer.addLayer(polylineBg);

                // Main route polyline with Route Violet (#6546AD)
                const polyline = L.polyline(latLngs, {
                    color: '#6546AD',
                    weight: 3.5,
                    opacity: 0.95,
                    dashArray: '8, 6',
                    lineJoin: 'round'
                });
                polylineLayer.addLayer(polyline);
            }

            // Fit map bounds
            if (latLngs.length > 0) {
                try {
                    const bounds = L.latLngBounds(latLngs);
                    map.fitBounds(bounds, { padding: [40, 40], maxZoom: 15 });
                } catch (e) {
                    console.warn("Could not fit bounds", e);
                }
            }
        },

        renderSelectableCatalog: function (containerId, allStops, selectedStopIds) {
            const instance = mapInstances[containerId];
            if (!instance) return;

            const { map, allStopsLayer } = instance;
            allStopsLayer.clearLayers();
            instance.markerMap = {};

            if (!allStops || allStops.length === 0) return;

            const selectedSet = new Set(selectedStopIds || []);
            const latLngs = [];

            allStops.forEach(stop => {
                if (stop.lat && stop.lon) {
                    const pos = [stop.lat, stop.lon];
                    latLngs.push(pos);

                    const isSelected = selectedSet.has(stop.id);

                    const iconHtml = isSelected
                        ? `<div class="flex items-center justify-center w-6 h-6 rounded-full bg-[#0B5F9B] text-white border-2 border-white shadow-md font-mono font-bold text-xs">✓</div>`
                        : `<div class="flex items-center justify-center w-5 h-5 rounded-full bg-white text-[#5A6275] border-2 border-[#9BA4B8] shadow-xs hover:border-[#0B5F9B] hover:scale-110 transition-transform"></div>`;

                    const customIcon = L.divIcon({
                        html: iconHtml,
                        className: 'yps-map-catalog-marker',
                        iconSize: isSelected ? [24, 24] : [20, 20],
                        iconAnchor: isSelected ? [12, 12] : [10, 10],
                        popupAnchor: [0, -10]
                    });

                    const marker = L.marker(pos, { icon: customIcon });

                    const statusBadge = isSelected
                        ? '<span class="inline-block px-1.5 py-0.5 rounded bg-[#DFF7EC] text-[#137455] text-2xs font-semibold">Included</span>'
                        : '<span class="inline-block px-1.5 py-0.5 rounded bg-[#F0F2FA] text-[#5A6275] text-2xs">Not in route</span>';

                    const popupContent = `
                        <div class="p-1 text-xs font-sans">
                            <div class="font-bold text-[#121521]">${stop.stopName}</div>
                            <div class="text-[#5A6275] mt-0.5">${stop.regionName || 'No Region'}</div>
                            <div class="mt-1 flex items-center justify-between gap-2">
                                ${statusBadge}
                                <span class="text-2xs font-mono text-[#9BA4B8]">${stop.lat.toFixed(4)}, ${stop.lon.toFixed(4)}</span>
                            </div>
                        </div>
                    `;
                    marker.bindPopup(popupContent);

                    marker.on('click', () => {
                        if (instance.dotNetHelper) {
                            instance.dotNetHelper.invokeMethodAsync('OnMapStopToggled', stop.id);
                        }
                    });

                    allStopsLayer.addLayer(marker);
                    instance.markerMap[stop.id] = marker;
                }
            });

            if (latLngs.length > 0) {
                try {
                    const bounds = L.latLngBounds(latLngs);
                    map.fitBounds(bounds, { padding: [30, 30], maxZoom: 14 });
                } catch (e) {
                    console.warn("Could not fit catalog bounds", e);
                }
            }
        },

        highlightStop: function (containerId, stopId) {
            const instance = mapInstances[containerId];
            if (!instance || !instance.markerMap[stopId]) return;

            const marker = instance.markerMap[stopId];
            marker.openPopup();
            instance.map.panTo(marker.getLatLng(), { animate: true, duration: 0.5 });
        },

        invalidateSize: function (containerId) {
            const instance = mapInstances[containerId];
            if (instance && instance.map) {
                setTimeout(() => {
                    instance.map.invalidateSize();
                }, 100);
            }
        },

        destroyMap: function (containerId) {
            if (mapInstances[containerId]) {
                try {
                    mapInstances[containerId].map.remove();
                } catch (e) {
                    console.warn("Error removing map", e);
                }
                delete mapInstances[containerId];
            }
        }
    };
})();
