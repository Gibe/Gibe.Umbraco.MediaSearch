# Gibe.Umbraco.MediaSearch

__IMPORTANT : This is a preview of a module, and is not intended for use on production environments__

## Installation instructions

Configure Moriyama Search

Add the following to the dashboard config

```
<section alias="StartupMedia2DashboardSection">
	<areas>
		<area>media2</area>
	</areas>
	<tab caption="Search">
		<control>/App_Plugins/MediaSearch/dashboard.html</control>
	</tab>
</section>
```

## Usage

There will be a new media dashboard which demonstrates the faceted search. 

A new datatype will also have been added called "Media Search Picker" you can use this in place of the usual Media Picker.
 