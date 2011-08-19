require 'albacore'

desc "Build all the code"
task :build => 'build:mount_files'

namespace :build do
  
  task :clean => ['build/nuget', 'dist'] do
    #rm_f FileList['build/nuget/*']
  end
  
  msbuild :compile => :clean do |msb|
    msb.properties :configuration => :Release
    msb.targets :Build
    msb.solution = 'ObjectWorkFlow.sln'
  end
  
  directory 'build/nuget'
  directory 'build/nuget/lib'
  directory 'dist'
  
  task :mount_files => [:compile, 'build/nuget', 'build/nuget/lib'] do
    cp 'compile/objectflow.stateful/bin/release/objectflow.core.dll', 'build/nuget/lib/objectflow.core.dll'
    cp 'compile/objectflow.stateful/bin/release/objectflow.core.xml', 'build/nuget/lib/objectflow.core.xml'
    cp 'compile/objectflow.stateful/bin/release/objectflow.stateful.dll', 'build/nuget/lib/objectflow.stateful.dll'
    cp 'compile/objectflow.stateful/bin/release/objectflow.stateful.xml', 'build/nuget/lib/objectflow.stateful.xml'
    cp 'LICENSE.txt', 'build/nuget/LICENSE.txt'
  end
  
end

desc "does everything for a release, aside from bumping the version"
task :default => :package

desc "Build the entire NuGet package, but don't upload it"
task :package => 'package:build_package'

namespace :package do 
  
  desc "create the nuspec file"
  nuspec :make_spec do |nu|
    nu.id = 'StatefulObjectflow'
    nu.version = get_version
    nu.authors = 'Tim Kellogg'
    nu.owners = 'Tim Kellogg'
    nu.description = "Objectflow is a library for creating simple, lightweight workflows for .NET applications"
    nu.summary     = "Objectflow is a library for creating simple, lightweight workflows for .NET applications"
    nu.language = 'en-US'
    nu.licenseUrl = 'https://github.com/tkellogg/objectflow/blob/master/LICENSE.txt'
    nu.projectUrl = 'https://github.com/tkellogg/objectflow'
    nu.working_directory = 'build/nuget'
    nu.output_file = 'stateful_objectflow.nuspec'
    nu.tags = 'workflow'
  end
  
  nugetpack :build_package => [:build, :make_spec, 'dist'] do |nu|
    nu.base_folder = 'build/nuget'
    nu.nuspec = 'build/nuget/stateful_objectflow.nuspec'
    nu.output = 'dist'
  end
  
end

desc "print out the current assembly version"
task :version do
  puts "Version is: #{get_version}"
end

# look at AssemblyInfo.cs and extract version
def get_version
  version = '0.5.0.0'
  File.open 'objectflow.stateful/Properties/AssemblyInfo.cs' do |f|
    txt = f.read
    if /\[assembly: AssemblyVersion\("([^"]+)"\)\]\s*/ =~ txt
      version = $~[1]
      puts "Version is #{version}"
    end
  end
  version
end

