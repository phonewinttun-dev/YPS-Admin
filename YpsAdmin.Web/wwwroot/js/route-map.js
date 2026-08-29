// Leaflet OpenStreetMap Interop for YPS Route Indicator UI (DESIGN.md semantic tokens)
(function () {
    const mapInstances = {};

    // Standard OpenStreetMap with complete Myanmar road networks, townships and street data
    const OSM_TILE_URL = 'https://tile.openstreetmap.org/{z}/{x}/{y}.png';
    const OSM_ATTRIBUTION = '&copy; <a href="https://www.openstreetmap.org/copyright" target="_blank" rel="noopener">OpenStreetMap</a> contributors';

    // Haversine formula to compute distance in km between two lat/lon coordinates
    function calculateDistanceKm(lat1, lon1, lat2, lon2) {
        const R = 6371; // Earth radius in km
        const dLat = (lat2 - lat1) * Math.PI / 180;
        const dLon = (lon2 - lon1) * Math.PI / 180;
        const a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                  Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
                  Math.sin(dLon / 2) * Math.sin(dLon / 2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    window.YpsRouteMap = {
        initMap: function (containerId, dotNetHelper, initialLat = 16.8661, initialLon = 96.1951, zoom = 12, isDark = true) {
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
                attributionControl: true,
                maxZoom: 19,
                minZoom: 6
            }).setView([initialLat, initialLon], zoom);

            // Add Zoom Control to bottom right for clean layout
            L.control.zoom({ position: 'bottomright' }).addTo(map);

            // OpenStreetMap tile layer with complete Myanmar street networks
            const tileLayer = L.tileLayer(OSM_TILE_URL, {
                maxZoom: 19,
                attribution: OSM_ATTRIBUTION
            }).addTo(map);

            // Create layer groups
            const routeMarkersLayer = L.layerGroup().addTo(map);
            const polylineLayer = L.layerGroup().addTo(map);
            const radarLayer = L.layerGroup().addTo(map);
            const allStopsLayer = L.layerGroup().addTo(map);

            mapInstances[containerId] = {
                map: map,
                tileLayer: tileLayer,
                dotNetHelper: dotNetHelper,
                routeMarkersLayer: routeMarkersLayer,
                polylineLayer: polylineLayer,
                radarLayer: radarLayer,
                allStopsLayer: allStopsLayer,
                markerMap: {},
                isDark: isDark,
                currentStops: []
            };

            setTimeout(() => {
                map.invalidateSize();
            }, 200);

            return true;
        },

        renderRoute: function (containerId, stops, isDark = null) {
            const instance = mapInstances[containerId];
            if (!instance) return;

            if (isDark !== null) {
                instance.isDark = isDark;
            }

            const { map, routeMarkersLayer, polylineLayer, radarLayer } = instance;
            routeMarkersLayer.clearLayers();
            polylineLayer.clearLayers();
            radarLayer.clearLayers();
            instance.markerMap = {};
            instance.currentStops = stops || [];

            if (!stops || stops.length === 0) {
                return;
            }

            const isDarkMode = instance.isDark;
            // DESIGN.md Colors: Bus Blue (#0B5F9B) in Light Mode, Brand Gold (#F6D867) in Dark Mode
            const accentColor = isDarkMode ? '#F6D867' : '#0B5F9B';
            const pinBg = isDarkMode ? '#121521' : '#FFFFFF';
            const pinText = isDarkMode ? '#F6D867' : '#0B5F9B';
            const pinBorder = isDarkMode ? '#F6D867' : '#0B5F9B';

            const latLngs = [];
            let cumulativeKm = 0;
            const stopDistances = [];

            stops.forEach((stop, index) => {
                if (stop.lat && stop.lon) {
                    const pos = [stop.lat, stop.lon];
                    latLngs.push(pos);

                    if (index > 0 && stops[index - 1].lat && stops[index - 1].lon) {
                        const segmentDist = calculateDistanceKm(stops[index - 1].lat, stops[index - 1].lon, stop.lat, stop.lon);
                        cumulativeKm += segmentDist;
                    }
                    stopDistances.push({ stopId: stop.busStopId, cumulativeKm: cumulativeKm });

                    const isFirst = index === 0;
                    const isLast = index === stops.length - 1;

                    // 1. Radar animation on Origin and Terminus endpoints
                    if (isFirst || isLast) {
                        const radarColor = accentColor;
                        const radarHtml = `
                            <div class="relative flex items-center justify-center w-16 h-16 pointer-events-none" style="transform: translate(-18px, -18px);">
                                <div class="absolute inset-0 rounded-full border border-[${radarColor}] opacity-50 radar-ring-1" style="border-color: ${radarColor};"></div>
                                <div class="absolute inset-0 rounded-full border border-[${radarColor}] opacity-30 radar-ring-2" style="border-color: ${radarColor};"></div>
                            </div>
                        `;
                        const radarIcon = L.divIcon({
                            html: radarHtml,
                            className: 'yps-map-radar-marker',
                            iconSize: [28, 28],
                            iconAnchor: [14, 14]
                        });
                        const radarMarker = L.marker(pos, { icon: radarIcon, interactive: false });
                        radarLayer.addLayer(radarMarker);
                    }

                    // 2. SVG Location Pointer Pins (Teardrop shape pointing directly to GPS coordinate, No # sign)
                    const pinWidth = (isFirst || isLast) ? 26 : 22;
                    const pinHeight = (isFirst || isLast) ? 33 : 28;
                    const pinAnchor = [pinWidth / 2, pinHeight];
                    const fontSize = (isFirst || isLast) ? '11px' : (stop.stopOrder >= 100 ? '8px' : '9.5px');
                    const zIndexOffset = isFirst ? 1000 : (isLast ? 999 : stop.stopOrder);

                    const iconHtml = `
                        <div class="relative flex flex-col items-center group cursor-pointer transition-transform duration-200 hover:scale-130 hover:z-50" style="transform-origin: bottom center;" title="Stop ${stop.stopOrder}: ${stop.stopName}">
                            <svg viewBox="0 0 24 30" width="${pinWidth}" height="${pinHeight}" style="filter: drop-shadow(0 2px 4px rgba(0,0,0,0.35)); overflow: visible;">
                                <path d="M12 0C5.373 0 0 5.373 0 12c0 8.5 12 18 12 18s12-9.5 12-18c0-6.627-5.373-12-12-12z" fill="${pinBg}" stroke="${pinBorder}" stroke-width="1.8" stroke-linejoin="round"/>
                                <circle cx="12" cy="11.5" r="8" fill="${accentColor}" opacity="0.12"/>
                                <text x="12" y="12" font-size="${fontSize}" font-family="JetBrains Mono, monospace" font-weight="700" fill="${pinText}" text-anchor="middle" dominant-baseline="central">${stop.stopOrder}</text>
                            </svg>
                            <div class="absolute bottom-full mb-1 whitespace-nowrap px-2 py-0.5 rounded bg-black/90 text-white border border-white/20 text-2xs font-mono opacity-0 group-hover:opacity-100 transition-opacity shadow-lg pointer-events-none z-50">
                                Stop ${stop.stopOrder}: ${stop.stopName} (${cumulativeKm.toFixed(1)} km)
                            </div>
                        </div>
                    `;

                    const customIcon = L.divIcon({
                        html: iconHtml,
                        className: 'yps-map-stop-marker',
                        iconSize: [pinWidth, pinHeight],
                        iconAnchor: pinAnchor,
                        popupAnchor: [0, -pinHeight]
                    });

                    const marker = L.marker(pos, { icon: customIcon, zIndexOffset: zIndexOffset });

                    const popupContent = `
                        <div class="p-1 text-xs font-sans">
                            <div class="flex items-center justify-between gap-2">
                                <span class="font-mono font-bold text-2xs px-2 py-0.5 rounded" style="background-color: ${accentColor}; color: ${isDarkMode ? '#000000' : '#FFFFFF'};">Stop ${stop.stopOrder}</span>
                                <span class="font-mono text-2xs opacity-75">${cumulativeKm.toFixed(1)} km from start</span>
                            </div>
                            <div class="font-bold text-sm mt-1" style="color: ${isDarkMode ? '#F7F8FF' : '#121521'};">${stop.stopName}</div>
                            <div class="text-xs opacity-75 mt-0.5">${stop.regionName || 'Yangon'}</div>
                            <div class="font-mono text-2xs opacity-60 mt-1">${stop.lat.toFixed(5)}° N, ${stop.lon.toFixed(5)}° E</div>
                        </div>
                    `;
                    marker.bindPopup(popupContent);

                    marker.on('click', () => {
                        if (instance.dotNetHelper) {
                            instance.dotNetHelper.invokeMethodAsync('OnMapStopClicked', stop.busStopId, cumulativeKm);
                        }
                    });

                    routeMarkersLayer.addLayer(marker);
                    instance.markerMap[stop.busStopId] = marker;
                }
            });

            // 3. Single-Accent Glowing Route Polyline (System Bus Blue in Light Mode, Brand Gold in Dark Mode)
            if (latLngs.length > 1) {
                // Ambient Soft Glow
                const polylineGlow = L.polyline(latLngs, {
                    color: accentColor,
                    weight: isDarkMode ? 12 : 8,
                    opacity: isDarkMode ? 0.25 : 0.2,
                    lineJoin: 'round',
                    lineCap: 'round'
                });
                polylineLayer.addLayer(polylineGlow);

                // Core Crisp Polyline
                const polylineCore = L.polyline(latLngs, {
                    color: accentColor,
                    weight: 3.5,
                    opacity: 0.95,
                    lineJoin: 'round',
                    lineCap: 'round'
                });
                polylineLayer.addLayer(polylineCore);
            }

            // Emit telemetry update to DotNet
            if (instance.dotNetHelper && stops.length > 0) {
                try {
                    instance.dotNetHelper.invokeMethodAsync('OnRouteTelemetryUpdated', cumulativeKm, JSON.stringify(stopDistances));
                } catch (e) { }
            }

            // Fit map bounds
            if (latLngs.length > 0) {
                try {
                    const bounds = L.latLngBounds(latLngs);
                    map.fitBounds(bounds, { padding: [50, 50], maxZoom: 16 });
                } catch (e) {
                    console.warn("Could not fit bounds", e);
                }
            }
        },

        renderSelectableCatalog: function (containerId, allStops, selectedStopIds, isDark = true) {
            const instance = mapInstances[containerId];
            if (!instance) return;

            const { map, allStopsLayer } = instance;
            allStopsLayer.clearLayers();
            instance.markerMap = {};

            if (!allStops || allStops.length === 0) return;

            const selectedSet = new Set(selectedStopIds || []);
            const latLngs = [];

            const isDarkMode = isDark !== null ? isDark : instance.isDark;
            const accentColor = isDarkMode ? '#F6D867' : '#0B5F9B';

            allStops.forEach(stop => {
                if (stop.lat && stop.lon) {
                    const pos = [stop.lat, stop.lon];
                    latLngs.push(pos);

                    const isSelected = selectedSet.has(stop.id);

                    const iconHtml = isSelected
                        ? `<div class="flex items-center justify-center w-5 h-5 rounded-full font-mono font-bold text-xs shadow-md border" style="background-color: ${accentColor}; color: ${isDarkMode ? '#000000' : '#FFFFFF'}; border-color: #ffffff;">✓</div>`
                        : `<div class="flex items-center justify-center w-3.5 h-3.5 rounded-full bg-white text-slate-600 border border-slate-400 shadow-xs hover:scale-125 transition-transform"></div>`;

                    const customIcon = L.divIcon({
                        html: iconHtml,
                        className: 'yps-map-catalog-marker',
                        iconSize: isSelected ? [20, 20] : [14, 14],
                        iconAnchor: isSelected ? [10, 10] : [7, 7],
                        popupAnchor: [0, -7]
                    });

                    const marker = L.marker(pos, { icon: customIcon });

                    const statusBadge = isSelected
                        ? `<span class="inline-block px-1.5 py-0.5 rounded font-semibold text-2xs" style="background-color: ${accentColor}; color: ${isDarkMode ? '#000000' : '#FFFFFF'};">Included</span>`
                        : '<span class="inline-block px-1.5 py-0.5 rounded bg-slate-200 text-slate-700 text-2xs">Not in route</span>';

                    const popupContent = `
                        <div class="p-1 text-xs font-sans">
                            <div class="font-bold">${stop.stopName}</div>
                            <div class="text-slate-400 mt-0.5">${stop.regionName || 'Yangon'}</div>
                            <div class="mt-1 flex items-center justify-between gap-2">
                                ${statusBadge}
                                <span class="text-2xs font-mono opacity-60">${stop.lat.toFixed(4)}, ${stop.lon.toFixed(4)}</span>
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
                    map.fitBounds(bounds, { padding: [30, 30], maxZoom: 15 });
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

        setTheme: function (containerId, isDark) {
            const instance = mapInstances[containerId];
            if (!instance) return;

            instance.isDark = isDark;
            // Re-render route with new theme colors
            if (instance.currentStops && instance.currentStops.length > 0) {
                this.renderRoute(containerId, instance.currentStops, isDark);
            }
        },

        fitBounds: function (containerId) {
            const instance = mapInstances[containerId];
            if (!instance || !instance.currentStops || instance.currentStops.length === 0) return;

            const latLngs = instance.currentStops
                .filter(s => s.lat && s.lon)
                .map(s => [s.lat, s.lon]);

            if (latLngs.length > 0) {
                try {
                    const bounds = L.latLngBounds(latLngs);
                    instance.map.fitBounds(bounds, { padding: [50, 50], maxZoom: 16 });
                } catch (e) { }
            }
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
